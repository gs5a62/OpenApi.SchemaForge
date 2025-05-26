# OpenApi Schema Tuner

A .NET helper library for generating OpenAPI schema definitions from fluent metadata or custom configurations.

## Features

- Describe enums in request parameter as user friendly list.
- Describe enums in request body schema.
- Describe enums in response body schema.
- Ability to add schema property description.
- Ability to add request parameter description.
- Ability to add controller description.
- Guest tag for endpoints with Anonymous authentication.
- Mark endpoint as deprecated with a message and clickable link to the new endpoint.
- Mark request parameter as deprecated with a message.
- Endpoint request examples.
- Normalized api paths with lowercase.
- Ability to group endpoints.
- Auto group deprecated endpoints.
- Ability to ignore request parameter.
- More will be implemented later.

## Getting Started

### Installation

You can install the package via NuGet Package Manager:

```bash
Install-Package OpenApiSchemaTuner.Net
```