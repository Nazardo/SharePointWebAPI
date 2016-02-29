# SharePoint.WebAPI
ASP.NET Web API integration into SharePoint 2013

## What is it?
It is a **SharePoint 2013** farm solution that enables **ASP.NET Web API** services to be hosted into a SharePoint web application.

## What is ASP.NET Web API?
ASP.NET Web API is a framework that makes it easy to build HTTP services that reach a broad range of clients,
including browsers and mobile devices.
ASP.NET Web API is an ideal platform for building RESTful applications on the .NET Framework.

* http://www.asp.net/web-api

## How does it work?
Web API features are activated via an *HTTP Module* that properly configures the hosting environment.
The module tries to mimic what normally is done into the Application_Start event of a simple ASP.NET application.
For this approach to work some internal classes from the System.Net.Http (opensourced by .NET team) have been included into the solution.

A reference to the module is added into the Web Application configuration (web.config) via a Web Application Feature.
The feature must be manually activated in Central Admin on the Web Application.
