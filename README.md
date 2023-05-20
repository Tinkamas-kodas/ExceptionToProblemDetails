# Versions

* 1.4.3 - Sample with mininal API without code generator- using endpoint attributes

* 1.4.2 - the MapToProblemDetails attribute could be placed on base class

* 1.4 - introduced attribute ExceptionMapClass

* 1.2 - removed reference to Microsoft.AspNetCore.Mvc.Core

# In English - [bellow](#Motivation)

# Motyvacija

Susieti OPEN API dokumentaciją su standartizuotų klaidų apdorojimu ProblemDetails.

**Pastaba** - turi būti kartu naudojamas su kokiu nors ProblemDetails paketu, pvz.:

* [Opw.HttpExceptions.AspNetCore](https://www.nuget.org/packages/Opw.HttpExceptions.AspNetCore/)

* [Hellang.Middleware.ProblemDetails](https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails/)

## Naudojimas

### 1.  Prisidėti paketą [package](https://www.nuget.org/packages/ExceptionToProblemDetails)

```sh
Install-Package ExceptionToProblemDetails
```

.NET Core command line interface:

```sh
dotnet add package ExceptionToProblemDetails
```

### 2. Prisidėti source code generatoriaus paketą [package](https://www.nuget.org/packages/ExceptionToProblemDetails.Generator)

```sh
Install-Package ExceptionToProblemDetails.Generator
```

.NET Core command line interface:

```sh
dotnet add package ExceptionToProblemDetails.Generator
```

### 3. Susikurti partial klasę ir dekoruoti ją atributu [ExceptionMapClass], kuri užregistruos middleware Exception mapper

Žemiau pateiktas pavyzdys, registruojantis Hellang.Middleware.ProblemDetails middleware

```c#
[ExceptionMapClass]
public partial class ExceptionToProblemDetailsMap
{
    private readonly ProblemDetailsOptions options;

    public ExceptionToProblemDetailsMap(ProblemDetailsOptions options)
    {
        this.options = options;
    }

    partial void MapConverter<TConverter, TException, TProblemDetails>(int statusCode, ExceptionToProblemDetails.ControllerActionDefinition actionDefinition) where TConverter : ExceptionToProblemDetails.IExceptionToProblemDetailsConverter<TException, TProblemDetails> where TException : System.Exception where TProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        var converter = Activator.CreateInstance<TConverter>();
        options.Map<TException>((context, exception)=>actionDefinition.MatchRoute(context.Request.RouteValues), 
            (context, exception) => converter.Convert(exception, statusCode));
    }
}
```

### 4. Dekoruoti controller klasės arba action metodus atributu [MapToProblemDetails]

```c#
[HttpGet("{location}")]
[ProducesResponseType(typeof(IEnumerable<WeatherForecast> ),StatusCodes.Status200OK )]
[MapToProblemDetails(StatusCodes.Status404NotFound, ExceptionType = typeof(NotFoundException))]
public IEnumerable<WeatherForecast> Get(string location)
{
```

  Source generatorius sugeneruos 3 žingsnyje apsirašytos partial klasės iškvietimus

### 5. Naudojant jums patinkantį ProblemDetails middleware užregistruoti Exception mapper

```c#
builder.Services.AddProblemDetails(options =>
{
    new ExceptionToProblemDetailsMap(options).Map();
    options.IncludeExceptionDetails = (context, exception) => false;
});
```

Pavyzdžiai pateikti [pavyzdiniai projektai](https://github.com/Tinkamas-kodas/ExceptionToProblemDetails/tree/main/demo)

<a id="Motivation"></a>

# Motivation

Tie the Open API documentation to standardized error handling with ProblemDetails.

Note - Must be used along with a ProblemDetails package such as:

* [Opw.HttpExceptions.AspNetCore] (https://www.nuget.org/packages/Opw.HttpExceptions.AspNetCore/)

* [Hellang.Middleware.ProblemDetails] (https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails/)

## Usage

### 1.  Add the [package](https://www.nuget.org/packages/ExceptionToProblemDetails)

```sh
Install-Package ExceptionToProblemDetails
```

.NET Core command line interface:

```sh
dotnet add package ExceptionToProblemDetails
```

### 2.  Add the source code generator [package](https://www.nuget.org/packages/ExceptionToProblemDetails.Generator)

```sh
Install-Package ExceptionToProblemDetails.Generator
```

.NET Core command line interface:

```sh
dotnet add package ExceptionToProblemDetails.Generator
```

### 3.  Create a partial class and decorate it with attribute [ExceptionMapClass], which will register the Exception mapper middleware

Below is an example registering the Hellang.Middleware.ProblemDetails middleware.

```c#
[ExceptionMapClass]
public partial class ExceptionToProblemDetailsMap
{
    private readonly ProblemDetailsOptions options;

    public ExceptionToProblemDetailsMap(ProblemDetailsOptions options)
    {
        this.options = options;
    }

    partial void MapConverter<TConverter, TException, TProblemDetails>(int statusCode, ExceptionToProblemDetails.ControllerActionDefinition actionDefinition) where TConverter : ExceptionToProblemDetails.IExceptionToProblemDetailsConverter<TException, TProblemDetails> where TException : System.Exception where TProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        var converter = Activator.CreateInstance<TConverter>();
        options.Map<TException>((context, exception)=>actionDefinition.MatchRoute(context.Request.RouteValues), 
            (context, exception) => converter.Convert(exception, statusCode));
    }
}
```

### 4.  Decorate the controller class or action methods with the [MapToProblemDetails] attribute.

```c#
[HttpGet("{location}")]
[ProducesResponseType(typeof(IEnumerable<WeatherForecast> ),StatusCodes.Status200OK )]
[MapToProblemDetails(StatusCodes.Status404NotFound, ExceptionType = typeof(NotFoundException))]
public IEnumerable<WeatherForecast> Get(string location)
{
```

The source generator will generate the calls described in step 3.

### 5.  Using your preferred ProblemDetails middleware, register the Exception mapper.

```c#

builder.Services.AddProblemDetails(options =>
{
    new ExceptionToProblemDetailsMap(options).Map();
    options.IncludeExceptionDetails = (context, exception) => false;
});
```

Examples can be found in the [sample project](https://github.com/Tinkamas-kodas/ExceptionToProblemDetails/tree/main/demo)