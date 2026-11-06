# README

The goal of this project is to automate repetitive processes and tasks. It allows you to create and configure your own processes, giving you flexibility over how tasks are executed.

Please use caution when defining processes, actions, and configurations, as they may affect your system or data.

## Process Automation

**Process Automation** is the main application for designing, configuring, and managing automated processes.

The project is structured around four main concepts: **Processes, Tasks, Steps, and Components.**

### Processes

Processes represent individual automations and are used to assign a unique name to each process.

### Tasks

Each process can contain one or multiple tasks. Every task is assigned an order number that determines its execution sequence.

### Steps

Steps define the actual actions performed by each task. This is where most of the automation logic is configured.

There are several types of components available, although they are currently in testing mode.

- You can define variables to store output values generated during execution.
- You can reference variables in the `Value` field by pressing the `@` key.
- You can use functions by pressing the `$` or `!` key.

### Components

Components provide specific functionality that can be used within task steps. They are the building blocks used to define what each step does.

## Process Viewer

**Process Viewer** is the application designed to execute and interact with processes previously created and compiled using **Process Automation**.

Unlike Process Automation, which focuses on designing and configuring processes, Process Viewer focuses on making those processes available for everyday use by end users.

It provides the following main features:

- **Process Execution:** Run previously configured and compiled processes.
- **Editable Variables:** Modify variable values to customize process behavior during execution without changing the original process definition.
- **Process Cloning:** Create copies of existing processes for independent use or customization.
- **Shortcut Creation:** Create shortcuts for quick and convenient access to frequently used processes.

## Project Status

**Please note:** This project is currently in the **alpha stage** and is still under active development.

If you think it could be useful to you, feel free to give it a try! Just keep in mind that some features may be incomplete or may not work as expected yet.

**Thank you for checking out this project!**