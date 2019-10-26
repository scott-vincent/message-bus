# message-bus
Using a message bus in C#

## Introduction

This is a simple example of a distributed system that communicates via a message bus and is asynchronous (message publish/subscribe). The message bus uses AWS SQS for persistence and AWS SNS for communication. This provides resilience as all services become loosly coupled and can function independently. If any service is not running its messages will be queued and can be processed later when the service starts. Timeouts can be configured so if messages sit on queues for too long they can be moved to error queues and processed accordingly (or re-added to the original queues when problems are resolved).

Queues are created for each message type, each publisher and each subscriber. This means there can be multiple subscribers to the same message type and each will get their own copy of the message when it is published. As the queues are persisted, even when a service goes down it is still subscribed to its messages so all the messages will still arrive when the service resumes (unless the message times out and gets moved to the error queue), i.e. no messages will be lost.

## Dependencies

goaws
JustEat/justsaying

## Instructions
