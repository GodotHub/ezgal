<div align="center" style="display:grid;place-items:center;">
        <p>
            <a href="https://gitee.com/cryingn/ezgal" target="_blank"><img width="180" src="./ezgal/image/icon.png" alt="ezgal logo"></a>
        <h1>ezgal</h1>
        </p>
        <img src="https://gitcode.com/godothub/ezgal/star/badge.svg?style=flat-square">
</div>

English|[中文](./README_CN.md)|[日本語](./README_JP.md)

This is a framework based on godot.mono designed to facilitate galgame development.

![](./image/image.png)

## Description

We initially implemented the **godot-ezgal** framework for Godot3 in November 2023, but discontinued maintenance due to scalability issues. The framework was subsequently rewritten. Here are the features of the new **ezgal** framework:

| Description          | ezgal                                                                                                                                                                                                 | Contributor |
| -------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------- |
| Godot Version        | Godot4 (with ongoing compatibility considerations)                                                                                                                                                    | cryingn     |
| Programming Language | C#(net8.0+)                                                                                                                                                                                           | cryingn     |
| Development Modes    | Supports two modes: [Deep Integration](#deep-integration) for single-file packaging with extended features, and [Low-Code Development](#low-code-development) enabling direct script editing via APIs | cryingn     |
| ezgal Interpreter    | Parses scenario files into JSON format and processes them segment by segment                                                                                                                          | cryingn     |
| Script Syntax        | Separates dialogue writing from scene direction for clear role differentiation                                                                                                                        | cryingn     |
| Script Language      | Chinese (Short-term support for Chinese/English grammar will be provided, with plans to support custom scripting languages in the future)                                                             | cryingn     |

## Usage

### Deep-Integration

Clone the source code into your project and import the **ezgal** folder into Godot:

```bash
git clone https://gitee.com/cryingn/ezgal
cd ezgal/ezgal
```

To package into a single executable, compile using the make folder into `./ezgal/code/FlowData.cs`. Packaging command:


```bash
dotnet run --project make
```


To revert to editable mode:


```bash
dotnet run --project make test
```


### Low-Code-Development

(Future mainstream development method - modify external folders for script/character art/dictionary/music editing)

Post-compilation, directly edit external folder contents for resource modification.

## Syntax

For intuitive scriptwriting, dialogue and scene direction are separated. Detailed below:
Dialogue

`Dialogue` Box format:

```
[对话框]
This demonstrates dialogue box display
Girl: This is how a character speaks in dialogue boxes
```


`Fullscreen` format:

```
[全屏]
Fullscreen supports multi-line content
But no character speaking
Supports up to 12 simultaneous lines
[全屏]
Redefine to switch line count early
```


`Options` share formatting with `Dialogue`/`Fullscreen` but store choices for player selection:

```
[选项]
{script:测试3.txt}Jump to Test3
{jump:循环1}Jump to Loop1
Continue
[全屏]
You'll see this after selecting fullscreen
```

BBCode supported for formatting. Define terms like:

```
[全屏]
This is a [url=专业名词]professional term[/url]
```

Selecting terms triggers dictionary popups. Define terms in dictionary folder via `专业名词.txt`.

### Scene Direction

Use `@` to mark jump points:

```
@循环1
This creates an infinite loop
{jump:循环1}
```

When jumping, it will return to the position of `@循环1`. Except for setting marked positions, all other presentation methods are uniformly managed using curly braces, as shown in the following examples:

```
{bg:封面.png}#Switch to cover
{少女:normal.png-1200x650-1.1}Girl: Hello everyone#Set character illustration
{script:测试2.txt, jump:循环1}#Jump to Loop 1 in the test2 script
```

The parameters within the curly braces are defined as follows:

| Parameter | Description                                                                                                                                                                                          | Example                      |
| --------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------- |
| bg        | Sets the background. The background image should be located in the `./image/background/` folder.                                                                                                     | bg:封面.png                    |
| script    | Jumps to the specified script position.                                                                                                                                                              | script:测试3.txt               |
| jump      | Jumps to the specified **marked position**.                                                                                                                                                          | jump:循环1                     |
| ef        | Intended for setting special effects, but not yet implemented.                                                                                                                                       |                              |
| [default] | Used to set character illustrations. [default] should have a corresponding name in the `./image/` folder, with the defined name format as `[image name]-[image height]x[image width]-[scale ratio]`. | girl:normal.png-1200x650-1.1 |


