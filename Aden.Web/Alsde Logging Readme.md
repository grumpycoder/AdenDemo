# Alsde.Mvc.Logging Readme

This library will configure logging for the MVC application. Configurations afforded are set in the appSettings node of the web.config file of your application. 

Database connection is named "LoggingContext" by default and use standard database connection string to your Microsoft SQL server. 
This has been added during installation of the nuget package. You can choose to change, but you must also change the key inside the AppSettings key of `LoggingContextName`. 
Otherwise, the logging library will be using the `LoggingContext` connection string key

~~~

<connectionStrings>
	<add name="LoggingContext" connectionString="Data Source=DatabaseInstanceName;initial catalog=DatabaseName;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False" providerName="System.Data.SqlClient" />
</connectionStrings>

~~~

Update this key if you are changing the `LoggingContext` connection string name. 

~~~

<appSettings>
   <add key="LoggingContextName" value="LoggingContext"/>
</appSettings>

~~~

The schema name of the logging tables can be defined with the `LoggingSchemaName` key. 
Default will be "dbo" if not specified. This has been added during install. 

~~~

<appSettings>
	<add key="LoggingSchemaName" value="LogSchemaName"/>
   <add key="LoggingContextName" value="LoggingContext"/>
</appSettings>

~~~


Logging tables can be created for you. This has been added during install.  


~~~

<appSettings>
	<add key="AutoCreateLoggingTables" value="true"/>
</appSettings>

~~~


Table Schema should be created as below. There must be a table created for "Perf", "Usage", "Error" and "Diagnostic". 

~~~

CREATE TABLE [dbo].[Perf](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime] NULL,
	[Product] [nvarchar](max) NULL,
	[Layer] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[Hostname] [nvarchar](max) NULL,
	[UserId] [nvarchar](max) NULL,
	[UserName] [nvarchar](max) NULL,
	[Exception] [nvarchar](max) NULL,
	[ElapsedMilliseconds] [int] NULL,
	[CorrelationId] [nvarchar](max) NULL,
	[CustomException] [nvarchar](max) NULL,
	[AdditionalInfo] [nvarchar](max) NULL,
 CONSTRAINT [PK_PerfLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

~~~


### Add global filter for tracking performance

Update your FilterConfig.cs file to add the TrackPerformance attributes to all controller actions like this: 

~~~

   public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(new TrackPerformanceAttribute(Constants.ApplicationName,
				Constants.LayerName));
		}
	}

~~~


You can add the performance tracker individually to controller actions instead of globally. 
Decorate your actions with the TrackPerformance attribute like this:

~~~

[TrackPerformance(Constants.ApplicationName, Constants.LayerName)]
public ActionResult Index()
{
	return View();
}

~~~

### Track Useage

Tracking usage of controller actions can be achieved by decorating the action with the TrackUsage decorator.

~~~

[TrackUsage(Constants.ApplicationName, Constants.LayerName, "View Home")]
public ActionResult Index()
{
	return View();
}

~~~

### Diagnostics 

Diagnostics can be added into your controller actions where you want to log diagnostic information. 


~~~

public ActionResult Contact()
{
	Helpers.LogWebDiagnostic(Constants.ApplicationName, Constants.LayerName,
		"Just checking in....",
		new Dictionary<string, object> { { "Very", "Important" } });
	  
	ViewBag.Message = "Your contact page.";

	return View();
}

~~~

Diagnostics require a key configured in the appSettings of the web.config to be enabled. 

~~~

<appSettings>
	<add key="EnableDiagnostics" value="true" />
</appSettings>

~~~


### Global handling application errors

To log errors globally add the following to the global.asax.cs code file. This code block
is set to redirect after logging to an ErrorController NotFound and Index actions. 

~~~

protected void Application_Error(object sender, EventArgs e)
{
	var ex = Server.GetLastError();
	if (ex == null)
		return;

	int httpStatus;
	string errorControllerAction;

	Helpers.GetHttpStatus(ex, out httpStatus);
	switch (httpStatus)
	{
		case 404:
			errorControllerAction = "NotFound";
			break;
		default:
			Helpers.LogWebError(Constants.ApplicationName, Constants.LayerName, ex);
			errorControllerAction = "Index";
			break;
	}

	var httpContext = ((MvcApplication)sender).Context;
	httpContext.ClearError();
	httpContext.Response.Clear();
	httpContext.Response.StatusCode = httpStatus;
	httpContext.Response.TrySkipIisCustomErrors = true;

	var routeData = new RouteData();
	routeData.Values["controller"] = "Error";
	routeData.Values["action"] = errorControllerAction;

	var controller = new ErrorController();
	((IController)controller)
		.Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
}

~~~
