//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Components;

//namespace Microsoft.AspNetCore.Components;

///// <summary>
///// Represents a subscription to the <c>OnPersisting</c> callback that <see cref="ComponentStatePersistenceManager"/> callback will trigger
///// when the application is being persisted.
///// </summary>
//public readonly struct PersistingComponentStateSubscription : IDisposable
//{
// private readonly List<PersistComponentStateRegistration>? _callbacks;
// private readonly PersistComponentStateRegistration? _callback;

// internal PersistingComponentStateSubscription(List<PersistComponentStateRegistration> callbacks, PersistComponentStateRegistration callback)
// {
//  _callbacks = callbacks;
//  _callback = callback;
// }

// /// <inheritdoc />
// public void Dispose()
// {
//  if (_callback.HasValue)
//  {
//   _callbacks?.Remove(_callback.Value);
//  }
// }
//}


//internal readonly struct PersistComponentStateRegistration(
//    Func<Task> callback,
//    IComponentRenderMode? renderMode)
//{
// public Func<Task> Callback { get; } = callback;

// public IComponentRenderMode? RenderMode { get; } = renderMode;
//}

///// <summary>
///// The state for the components and services of a components application.
///// </summary>
//public class PersistentComponentState
//{
// private IDictionary<string, byte[]>? _existingState;
// private readonly IDictionary<string, byte[]> _currentState;

// private readonly List<PersistComponentStateRegistration> _registeredCallbacks;

// public PersistentComponentState()
// {

// }
// internal PersistentComponentState(
//     IDictionary<string, byte[]> currentState,
//     List<PersistComponentStateRegistration> pauseCallbacks)
// {
//  _currentState = currentState;
//  _registeredCallbacks = pauseCallbacks;
// }

// internal bool PersistingState { get; set; }

// internal void InitializeExistingState(IDictionary<string, byte[]> existingState)
// {

// }

// /// <summary>
// /// Register a callback to persist the component state when the application is about to be paused.
// /// Registered callbacks can use this opportunity to persist their state so that it can be retrieved when the application resumes.
// /// </summary>
// /// <param name="callback">The callback to invoke when the application is being paused.</param>
// /// <returns>A subscription that can be used to unregister the callback when disposed.</returns>
// public PersistingComponentStateSubscription RegisterOnPersisting(Func<Task> callback)
//     => RegisterOnPersisting(callback, null);

// /// <summary>
// /// Register a callback to persist the component state when the application is about to be paused.
// /// Registered callbacks can use this opportunity to persist their state so that it can be retrieved when the application resumes.
// /// </summary>
// /// <param name="callback">The callback to invoke when the application is being paused.</param>
// /// <param name="renderMode"></param>
// /// <returns>A subscription that can be used to unregister the callback when disposed.</returns>
// public PersistingComponentStateSubscription RegisterOnPersisting(Func<Task> callback, IComponentRenderMode? renderMode)
// {
//  ArgumentNullException.ThrowIfNull(callback);

//  var persistenceCallback = new PersistComponentStateRegistration(callback, renderMode);

//  _registeredCallbacks.Add(persistenceCallback);

//  return new PersistingComponentStateSubscription(_registeredCallbacks, persistenceCallback);
// }


// public void PersistAsJson<TValue>(string key, TValue instance)
// {

// }

// /// <summary>
// /// Tries to retrieve the persisted state as JSON with the given <paramref name="key"/> and deserializes it into an
// /// instance of type <typeparamref name="TValue"/>.
// /// When the key is present, the state is successfully returned via <paramref name="instance"/>
// /// and removed from the <see cref="PersistentComponentState"/>.
// /// </summary>
// /// <param name="key">The key used to persist the instance.</param>
// /// <param name="instance">The persisted instance.</param>
// /// <returns><c>true</c> if the state was found; <c>false</c> otherwise.</returns>
// [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
// public bool TryTakeFromJson<TValue>(string key, [MaybeNullWhen(false)] out TValue? instance)
// {
//  instance = default;
//  return false;
// }

// private bool TryTake(string key, out byte[]? value)
// {
//  ArgumentNullException.ThrowIfNull(key);

//  if (_existingState == null)
//  {
//   // Services during prerendering might try to access their state upon injection on the page
//   // and we don't want to fail in that case.
//   // When a service is prerendering there is no state to restore and in other cases the host
//   // is responsible for initializing the state before services or components can access it.
//   value = default;
//   return false;
//  }

//  if (_existingState.TryGetValue(key, out value))
//  {
//   _existingState.Remove(key);
//   return true;
//  }
//  else
//  {
//   return false;
//  }
// }
//}
