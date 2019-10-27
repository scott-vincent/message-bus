# message-bus
Using a message bus in C#

## Introduction

This is a simple example of a distributed system that communicates via a message bus and is asynchronous (message publish/subscribe). The message bus uses AWS SQS for persistence and AWS SNS for communication. This provides resilience as all services become loosly coupled and can function independently. If any service is not running its messages will be queued and can be processed later when the service starts. Timeouts can be configured so if messages sit on queues for too long they can be moved to error queues and processed accordingly (or re-added to the original queues when problems are resolved). The system is also scalable because you can run multiple copies of a service and they will just pull messages from the same queues and share the workload.

Queues are created for each message type, each publisher and each subscriber. This means there can be multiple subscribers to the same message type and each will get their own copy of the message when it is published. As the queues are persisted, even when a service goes down it is still subscribed to its message queues so all the messages will still arrive when the service resumes (unless a message times out and gets moved to the error queue), i.e. no messages are lost.

## Dependencies

Name|Location|Description|Info
----|--------|-----------|----
Visual Studio 2019|https://visualstudio.microsoft.com/vs/|The IDE to build JustSaying and this sample|Supports .NET Core 3 (don't need Preview version)
.NET Core 3.0 SDK|https://dotnet.microsoft.com/download/dotnet-core/3.0|The latest version of .NET Core Runtime and SDK
Go Lang|https://golang.org/|The command-line environment to build goaws|Not needed if you want to use real AWS SNS/SQS
goaws|https://github.com/p4tin/goaws| An AWS SNS/SQS clone that runs locally|Written in Go
justeat/JustSaying|https://github.com/justeat/JustSaying|A light-weight message bus on top of AWS services (SNS and SQS)|Written in C#
AWS CLI|https://aws.amazon.com/cli/|Optional but useful for showing SNS topics and SQS queues|Works with our clone (goaws) service

## Sample

The sample currently provided with JustSaying is too simplistic and only allows one subscriber per publisher (subscribers use the same queue!). I wanted a more realistic sample where there are multiple subscribers to the same message type. The lack of documentation in JustSaying made this seemingly simple task a lot more complicated as it was not obvious how to configure JustSaying to produce the desired results. I also wanted to document full step-by-step instructions on how to download everything required, how to configure everything and how to get the more realistic sample up and running from scratch.

I've taken the kitchen/orderingApi sample from JustSaying and expanded it. The modified version is in this repo along with a yaml file for configuring goaws.

## Instructions

1. Download and install Visual Studio, .NET Core SDK, Go Lang and AWS CLI.
2. Check that everything is installed ok. Some of the installers update the PATH so make sure you open a new Command Prompt window after all the installers have run. Enter the following commands:
````
    go version
    aws --version
````
3. As we are not using real AWS you can configure AWS CLI with dummy credentials. Run "aws configure" and set the following:
````
    AWS Access Key ID: dummy
    AWS Secret Access Key: dummy
    Default region name: eu-west-1
    Default output format: json
````
4. Download and build goaws. Enter the following commands:
````
    go get github.com/p4tin/goaws/...
    cd C:\users\<your_user>\go\src\github.com\p4tin\goaws
    go build -o app/goaws.exe app/cmd/goaws.go
````
5. Configure and run goaws. There is a YAML file that configures goaws and you specify which profile you want on the command line. You can either replace the YAML file with my version or just append my version to the end of the original. Note that goaws runs on port 4100. If you want it to run on a different port just modify the YAML file accordingly. Enter the following commands:
````
    cd C:\users\<your_user>\go\src\github.com\p4tin\goaws
    copy <this repo>\goaws\conf\goaws.yaml conf\
    goaws kitchen-orders
````
6. Download JustSaying and build NuGet packages. Enter the following commands:
````
    cd C:\users\<your_user>\<your_visualstudio_workspace>
    git clone https://github.com/justeat/JustSaying.git
    powershell -ExecutionPolicy ByPass -File JustSaying\build.ps1
````
## Notes

The sample assumes you are using goaws rather than real AWS services. If you are using real AWS you need to change some of the configuration files and tweak the source code (see the comments in the source).
