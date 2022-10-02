# General overview

Welcome to documentation page of Raven.NET framework. On this page you will find general overview of possibilities that library can provide, and additionally you can check some hyperlinks that will redirect you to page that may give you more detailed information about spefiic topic.

## Flow Diagram
The diagram below is to give a little bit of understanding what the way system is prepared to work. This should also describe core differences between default watcher and type watcher.

![image info](./Images/flow.drawio.png)


## Use cases

Animal refernece makes it easy to relate between code and real world perspective. It makes nice use of publisher/subscriber template and additionally, it encapsulates SOLID principles as you can easily extend already builded scenario without modifing old code, and you can create "army" of ravens that will have its own single responsibility. Animal reference should also make you possible to guess some use cases, but then in this page we will describe some that we can think about.

### Audit trails

For implement audit services, when you want to keep track of specific objects. Each time you need to know how objects are changing within the time, Then Raven.NET is valid choice for you. This library will allow you to have one to many reaction, and you can decide how many watchers you want to have, and you can react on changes differently. This allowes you to react on one change in different way that you will react to another. For audit specific case its also good that library does not overwrite watched object. Then you can be sure that you have clean audit that only provide solution for your specific case.

### Simple event source

### Notification services

### Lose coupling between objects

## Follow up documents


