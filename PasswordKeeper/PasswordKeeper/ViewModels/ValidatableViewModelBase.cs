using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;

namespace PasswordKeeper.ViewModels;

/// <summary>
/// Base class bridging a FluentValidation validator to <see cref="INotifyDataErrorInfo"/>,
/// so Avalonia renders rule failures under the bound field.
/// Rules live in a dedicated validator class; the view model holds no validation attributes.
/// Errors surface only for properties the user has actually changed, so a fresh form
/// doesn't open covered in red; <see cref="IsValid"/> gates commands against all rules.
/// </summary>
public abstract class ValidatableViewModelBase<T> : ViewModelBase, INotifyDataErrorInfo
    where T : ValidatableViewModelBase<T>
{
    private readonly IValidator<T> _validator;
    private readonly Dictionary<string, ValidationResult[]> _errors = [];
    private readonly Dictionary<string, string> _manualErrors = [];
    private readonly HashSet<string> _touched = [];

    protected ValidatableViewModelBase(IValidator<T> validator)
    {
        _validator = validator;
        PropertyChanged += OnAnyPropertyChanged;
    }

    /// <summary>
    /// Runs the validator without surfacing errors — for CanExecute checks.
    /// </summary>
    protected bool IsValid => _validator.Validate((T)this).IsValid;

    /// <summary>
    /// Attaches an operation result to the field it belongs to (e.g. wrong master password).
    /// Passing null clears it; editing the field clears it automatically.
    /// </summary>
    protected void SetError(string propertyName, string? message)
    {
        if (message is null)
            _manualErrors.Remove(propertyName);
        else
            _manualErrors[propertyName] = message;

        RefreshErrors();
    }

    public bool HasErrors => _errors.Count > 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) =>
        propertyName is { } name && _errors.TryGetValue(name, out var errors) ? errors : [];

    private void OnAnyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not { } name || name == nameof(HasErrors))
            return;

        // Editing a field drops its pending operation error and re-runs validation
        _manualErrors.Remove(name);
        _touched.Add(name);
        RefreshErrors();
    }

    private void RefreshErrors()
    {
        // Whole-object validation keeps cross-field rules (e.g. password match) in sync
        // no matter which side of the pair changed
        var failures = _validator
            .Validate((T)this)
            .Errors.Where(failure => _touched.Contains(failure.PropertyName))
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray()
            );

        foreach (var (name, message) in _manualErrors)
            failures[name] = [.. failures.GetValueOrDefault(name, []), message];

        var hadErrors = HasErrors;
        foreach (var name in _errors.Keys.Union(failures.Keys).ToArray())
        {
            var oldMessages = _errors.TryGetValue(name, out var old)
                ? old.Select(error => error.ErrorMessage)
                : [];
            var newMessages = failures.GetValueOrDefault(name, []);

            if (oldMessages.SequenceEqual(newMessages))
                continue;

            if (newMessages.Length == 0)
                _errors.Remove(name);
            else
                _errors[name] = [.. newMessages.Select(message => new ValidationResult(message))];

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(name));
        }

        if (hadErrors != HasErrors)
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasErrors)));
    }
}
