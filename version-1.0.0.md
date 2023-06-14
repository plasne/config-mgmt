# Version 1.0.0

IGetter

## Config

The `Config` class provides a way to create `Entity` instances. It also provides functionality that can be run across all `Entity` instances. We would expect the `Config` class to be a singleton and be registered via dependency injection or available via a global variable.

To create a new `Entity`, call one of the following methods:

- `AsString(<key>)`: This returns a `Entity` instance that can be used to get, validate, conform, resolve, etc. configuration values into a string (ex. "simple").

- `AsStrings(<key>)`: This returns a `Entity` instance that can be used to get, validate, conform, resolve, etc. configuration values into a list of strings (ex. "one", "two", "three").

- `AsInteger(<key>)`: This returns a `Entity` instance that can be used to get, validate, conform, resolve, etc. configuration values into an integer (ex. 7).

- `AsRational(<key>)`: This returns a `Entity` instance that can be used to get, validate, conform, resolve, etc. configuration values into a number that could possibly contain a fractional component (ex. 12 or 12.4).

- `AsBoolean(<key>)`: This returns a `Entity` instance that can be used to get, validate, conform, resolve, etc. configuration values into a boolean (ex. TRUE or FALSE).

- `AsEnum(<key>)`: This returns a `Entity` instance that can be used to get, validate, conform, resolve, etc. configuration values into an enumerable value (ex. one of "red", "blue", or "green"). `AsEnum` will need some way to designate the enumerable used or possible values, but this will likely vary considerably for each language (even how an enumerable is represented is highly language dependent).

In all of the above, `<key>` is a string that should uniquely identify this configuration value.

The `Config` implementation should track the `Entity` instances that are created so that they can be used for the following functionality:

- `Validate()`: This method validates all `Entity` instances that have been created using this `Config`. Validation of an `Entity` will fail if `Require()` and `HasValue` is FALSE. Use of `SetTransform()` could ensure that values are not set for undesireable data. Ideally, the implementation of `Validate()` would return a "result" or "error" encapsulating everything that is wrong, however, implementations may simply throw an exception if that is more appropriate for the language.

## Entity

The `Entity` class provides a way to get, validate, conform, resolve, etc. configuration values. It is created by calling one of the methods on the `Config` class. It is not expected that `Entity` instances will be created directly (ideally the implementation would prevent this).

The following properties are available on the `Entity` class:

- `Key` (GET string): When creating an `Entity` instance using one of the methods on the `Config` class, the value passed to the method becomes the Key. This should be a unique identifier for the configuration value.

- `Value` (GET T): This gets the value of the configuration value. The type (T) of the value is dependent on the method used to create the `Entity` instance. This is a property because it is the expected last call of the Entity chain (more on this below).

- `HasValue` (GET boolean): TRUE if a value has been determined for this configuration value, FALSE otherwise.

- `IsRequired` (GET boolean): TRUE if this configuration value is required, FALSE otherwise.

The following methods are available on the `Entity` class:

- `TryGet(<optional-key>)`: This method attempts to get the value of the configuration value. If the optional key is provided (string), it will attempt to get the value from the `Entity` instance with the provided key. If the optional key is not provided, it will attempt to get the value from the `Entity` instance with the `Key` property of this `Entity` instance.

    `TryGet()` should work by attempting to get a value from each Getter given a key.

    Multiple `TryGet()` calls can be specified. Internally, each call should record a value 

All of the above methods return the `Entity` instance. This allows the user to chain the methods together (typically ending in Value). For example:

```csharp
string serviceUrl = config.AsString("SERVICE_URL").TryGet().TryGet("BASE_URL").Require().Log().Value;
```

The above example does the following:
1. Gets the `Entity` instance for a configuration value identified "SERVICE_URL". It will be a string.
1. Attempts to get the value by calling `TryGet()` looking for a key of "SERVICE_URL" across all Getters.
1. Attempts to get the value by calling `TryGet()` looking for a key of "BASE_URL" across all Getters.
1. Requires that a value has been set for this configuration value.
1. Logs the value of this configuration value.
1. Gets the value of this configuration value.