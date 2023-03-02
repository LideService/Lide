**Main problems behind the idea:**

- Debugging a remote inaccessible environment of distributed services: Let's say you have a micro-service architecture with several interconnected services, and you're experiencing an issue in production. Using the tool, you can record the request that's causing the problem and replay it in a test environment, allowing you to debug the issue in a controlled environment without having to reproduce the entire system state in production. This can be especially helpful if the problem only occurs under specific circumstances that are difficult to replicate in a local environment.

- Communicating reproducible scenarios between business and developers: In a complex system, it can be difficult to communicate the exact scenario that's causing a problem between business stakeholders and developers. With the tool, you can record the request that's causing the problem and share it with the relevant stakeholders, allowing them to see exactly what's happening and facilitating communication between all parties involved.

- Debugging automation tests spanning multiple micro-services: In a micro-service architecture, it's common to have automation tests that span multiple services. If a test fails, it can be difficult to determine exactly where the problem is occurring. Using the tool, you can record the test execution and replay it, allowing you to quickly pinpoint the exact location where the test failed.

- Propagating data through HTTP both ways: In a micro-service architecture, it's often necessary to propagate data through multiple services. With the tool, you can decorate any or all types within an application automatically, allowing you to propagate data through HTTP both ways, request and response, through multiple services within a micro-services environment. This can be especially helpful if you need to pass information such as authentication tokens or other context between services.

Overall, the tool provides a way to more easily debug and test distributed systems, facilitating communication between business stakeholders and developers and enabling more efficient collaboration between different teams within an organization.

In case of a problem, the order of failing tests should be:
- specific service unit test
- specific service integration test
- whole system integration test

But in the real world, there might be a scenarios where the first two failure points, doesn't detect any problems. Or maybe due to specific, the tests rely on live data. Whatever the case is, if a only a system test fails, the debugging process is a nightmare. 

- rollback current service changes
- rollback any top level services changes
- continue to rollback, until a commit is found where the test stops to fail

But in case of relying on live data which has changed, there would be a lot of wasted time and nothing would be found in the end.

A recording of the tests execution can be made at the point where all were passing. Then in case a test fails, whatever the reason, the tool can provide with fast reasoning, where the first difference in the execution data occurred.

For example:

- **expected:** 
  - Service: <ServiceName>
  - File: FileFacade.cs
  - Line: 27
  - Method: bool Exist(string filePath)
  - Input arguments: "/tmp/someTmpDir/expectedFile"
  - Result: true
  - StackTrace: <Full stack trace here. Including the services call chain if needed>
- **actual:**
  - Service: <ServiceName>
  - File: FileFacade.cs
  - Line: 27
  - Method: bool Exist(string filePath)
  - Input arguments: "/tmp/someTmpDir/expectedFile"
  - Result: false
  - StackTrace: <Full stack trace here. Including the services call chain if needed>

Thus giving the exact location and the exact reason as to which the test failed, instantly.

Enables the following:
- Allows for decoration of any, or all, types within application automatically with the ability to be applied to single request.
- Can propagate data through http both ways, request and response, through multiple services within micro-services environment.

Implementations:
- Ability to record and replay a specific request allowing time-travel debugging, debugging request of remote environments as they were.
- Also can be used to easily find exact location where a difference occurred in automation test spanning multiple micro-services.

Requirements:
- Dependency injection with proper isolation - All system calls have isolation.
- Also depends on HttpClient to be resolved through the DI - or IHttpClientFacory.

Notes:
- Currently the serialization uses the native BinaryFormatter - obsolete and possibly unsafe if the processes is used on non-trusted records. 
- It can be replaced with any serializer capable of reading and writing private and static fields and members.
