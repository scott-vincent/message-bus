# message-bus
Using a message bus in C#

## Introduction

This is a simple example of a distributed system that communicates via a message bus and is asynchronous (message publish/subscribe). The message bus uses AWS SQS for persistence and AWS SNS for communication. This provides resilience as all services become loosly coupled and can function independently. If any service is not running its messages will be queued and can be processed later when the service starts. Timeouts can be configured so if messages sit on queues for too long they can be moved to error queues and processed accordingly (or re-added to the original queues when problems are resolved). The system is also scalable because you can run multiple copies of a service and they will just pull messages from the same queues and share the workload.

Queues are created for each message type, each publisher and each subscriber. This means there can be multiple subscribers to the same message type and each will get their own copy of the message when it is published. As the queues are persisted, even when a service goes down it is still subscribed to its message queues so all the messages will still arrive when the service resumes (unless a message times out and gets moved to the error queue), i.e. no messages will be lost.

## Dependencies

Name|Location|Description|Info
----|--------|-----------|----
goaws|https://github.com/p4tin/goaws| An AWS SNS/SQS clone that runs locally|Written in Go
justeat/JustSaying|https://github.com/justeat/JustSaying|A light-weight message bus on top of AWS services (SNS and SQS)|Written in C#

## Sample

The sample currently provided with JustSaying is too simplistic and only allows one subscriber per publisher (subscribers use the same queue!). I wanted a more realistic sample where there are multiple subscribers to the same message type. The lack of documentation in JustSaying made this seemingly simple task a lot more complicated as it was not obvious how to configure JustSaying to produce the desired results. I also wanted to document full step-by-step instructions on how to download everything required, how to configure everything and how to get the more realistic sample up and running from scratch.

## Instructions

