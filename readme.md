Main problems behind the idea:
    1. Debug a remote inaccessible environment of distributed services - or micro-services.
    2. Easy way to communicate reproducible scenarios between business (PM/PO) and developers.

    3. In distributed or micro-service architecture in best case, everything is properly covered with automation tests.
In case of a problem, the order of failing tests should be:
    - specific service unit test
    - specific service integration test
    - whole system integration test
But in the real world, there might be a scenarios where the first two failure points, doesn't detect any problems.
Or maybe due to specific, the tests rely on live data.
Whatever the case is, if a only a system test fails, the debugging process is a nightmare. 
    - rollback current service changes
    - rollback any top level services changes
    - continue to rollback, until a commit is found where the test stops to fail
But in case of relying on live data which has changed, there would be a lot of wasted time and nothing would be found in the end.
A recording of the tests execution can be made at the point where all were passing.
Then in case a test fails, whatever the reason, the tool can provide with fast reasoning, where the first difference in the execution data occurred.
For ex.:
    - expected: 
        Service: <ServiceName>
        File: FileFacade.cs
        Line: 27
        Method: bool Exist(string filePath)
        Input arguments: "/tmp/someTmpDir/expectedFile"
        Result: true
        StackTrace: <Full stack trace here. Including the services call chain if needed>
    - actual:
        Service: <ServiceName>
        File: FileFacade.cs
        Line: 27
        Method: bool Exist(string filePath)
        Input arguments: "/tmp/someTmpDir/expectedFile"
        Result: false
        StackTrace: <Full stack trace here. Including the services call chain if needed>
Thus giving the exact location and the exact reason as to which the test failed, instantly.


Enables the following:
    Allows for decoration of any, or all, types within application automatically with the ability to be applied to single request.
    Can propagate data through http both ways, request and response, through multiple services within micro-services environment.

Implementations:
    Ability to record and replay a specific request allowing time-travel debugging, debugging request of remote environments as they were.
    Also can be used to easily find exact location where a difference occurred in automation test spanning multiple micro-services.

Requirements:
    Dependency injection with proper isolation - All system calls have isolation.
    Also depends on HttpClient to be resolved through the DI - or IHttpClientFacory.

Notes:
    Currently the serialization uses the native BinaryFormatter - obsolete and possibly unsafe if the processes is used on non-trusted records. 
    It can be replaced with any serializer capable of reading and writing private and static fields and members.




A

