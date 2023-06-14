# Configuration Management

Almost every component of every service requires configuration. There are many many methods to configuring a solution, but there are some important tenants that should be observed:

1. __Configuration should be validated and conformed on startup.__ It is beneficial to terminate the execution of our service during startup when it has an invalid configuration, before it starts processing requests. When validating, the configuration should also be conformed to the expected datatypes and ranges. When the configuration value do not conform, it should fail validation or use a default.

1. __Configuration should be logged on startup.__ When troubleshooting issues with the service, it can be invaluable to look at the startup logs to see how the service was configured. While we do not want to show secrets, it is still important to show whether the value is set or not.

1. __Configuration should support secrets in a safe way.__ This solution should enable developers to store secrets easily and safely. It should make it easy to do the right thing.

1. __Configuration should be easy for administrators to set properly.__ There are several considerations here:
    - Configuration values should have reasonable defaults whenever possible.
    - Very specific configuration values may derive from more generic configuration values. For instance, if there are 4 places in the code that need a retry interval, consider using 4 configuration values that all default to using the value of a single configuration value.
    - Consider allowing for "templates" or "modes" that can be set as a single configuration value that sets many other configuration values.
    - Configuration values can interact with one another. For instance, if one value is `true`, other values may be required. These more complex validations should be enforced.
    - Configuration should be documented extensively and clearly.

## Why Implement a Configuration Management Solution?

There are some configuration management solutions available, but it is rare to find a solution that meets all of the tenants above. No cross-platform, cross-language solution has been found that provides consistency and meets the requirements.

## Contribution

This documentation will serve as a schematic for a cross-platform, cross-language solution. As CSE developers have a need for a configuration management solution in a particular language, please contribute to this repository with a solution that adheres to the schematic.

The schematic will be versioned so that it can be clear to users of this solution which features are available in which versions.

## Schematics

The schematics will serve as a guide on how to implement the solution across languages. In general, capabilities and syntax should be as consistent as possible. It will not address how components are registered for use (ex. dependency injection) or how they are configured (ex. the URL of an AppConfig instance) as these can vary per language. There will be language-specific guides for these topics.

- [Version 1.0.0](./version-1.0.0.md)