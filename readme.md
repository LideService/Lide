[What it is](#what-it-is)
[What it solves](#what-it-solves)
[What it can do](#what-it-can-do)
[Example scenario](#example-scenario)
[Requirements](#requirements)
[Limitations](#limitations)
[Installation](#installation)
[Configuration](#configuration)

### What it is

The tool provides a way to more easily debug and test distributed systems, facilitating communication between business stakeholders and developers and enabling more efficient collaboration between different teams within an organization.

No need to decorate each class or function with attributes, use code weavers or change the application behavior in any way. The functionality can be enabled on per request basis which will affect only the given request performance. Which in live environment, with multiple concurrent requests, will record just the given request in isolation, without affecting the performance or response time for any other request.

In case a runtime decorator, has an error within it will not affect the processing of the request or the end result. That is, if the problem is not is not blocking - for example infinite wait, or infinite loop, or... `System.Environment.Exit(1);`

### What it solves

- Debugging a remote inaccessible environment of distributed services: Let's say you have a micro-service architecture with several interconnected services, and you're experiencing an issue in production. Using the tool, you can record the request that's causing the problem and replay it in a test environment, allowing you to debug the issue in a controlled environment without having to reproduce the entire system state in production. This can be especially helpful if the problem only occurs under specific circumstances that are difficult to replicate in a local environment.

- Communicating reproducible scenarios between business and developers: In a complex system, it can be difficult to communicate the exact scenario that's causing a problem between business stakeholders and developers. With the tool, you can record the request that's causing the problem and share it with the relevant stakeholders, allowing them to see exactly what's happening and facilitating communication between all parties involved.

- Debugging automation tests spanning multiple micro-services: In a micro-service architecture, it's common to have automation tests that span multiple services. If a test fails, it can be difficult to determine exactly where the problem is occurring. Using the tool, you can record the test execution and replay it, allowing you to quickly pinpoint the exact location where the test failed.

- Propagating data through HTTP both ways: In a micro-service architecture, it's often necessary to propagate data through multiple services. With the tool, you can decorate any or all types within an application automatically, allowing you to propagate data through HTTP both ways, request and response, through multiple services within a micro-services environment. This can be especially helpful if you need to pass information such as authentication tokens or other context between services.

### What it can do

- Allows for decoration of any, or all, types within application automatically with the ability to be applied to single request.
- Can propagate data through http both ways, request and response, through multiple services within micro-services environment.
- Ability to record and replay a specific request allowing time-travel debugging, debugging request of remote environments as they were.
- Also can be used to easily find exact location where a difference occurred in automation test spanning multiple micro-services.

### Example scenario

In case of a bug, the order of failing tests should be:
- specific service unit test
- specific service integration test
- whole system integration test

But in the real world, there might be a scenarios where the first two failure points, doesn't detect any problems. Or maybe due to specifics, the tests rely on live data. Whatever the case is, if a only a system test fails, the debugging process is a nightmare. 

- rollback current service changes
- rollback any top level services changes
- continue to rollback, until a commit is found where the test stops to fail

In the case of relying on live data which has changed, there would be a lot of wasted time and nothing would be found in the end. But if a recording of the tests execution was made at the point where all were passing, whenever a test fails, whatever the reason, the tool can provide with fast reasoning, where the first difference in the execution data occurred.

### Requirements
- Dependency injection with proper isolation - All system calls have isolation.
- Also depends on HttpClient to be resolved through the DI - or IHttpClientFactory.

### Limitations
- Currently the serialization uses the native BinaryFormatter. - obsolete and possibly unsafe if the processes is used on non-trusted records. 
- It is protected with encryption, but due to being obsolete and unsafe it is not recommended to be used with non-trusted records.
- Also can be replaced with any serializer, as long as it is capable of reading and writing private and static fields and members, streams, types, etc - basically everything.
- Also the following scenarios are not supported yet:
  - functions returning ref: `public ref string GetStringRef()`
  - functions having ref parameter: `public void ProcessVariable(ref string data)`
  - functions having out parameter: `public void SetOutParam(out int fieldToSet)`


