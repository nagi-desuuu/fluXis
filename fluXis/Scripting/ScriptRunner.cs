﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NLua;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace fluXis.Scripting;

public class ScriptRunner
{
    protected static Logger Logger { get; } = Logger.GetLogger("scripting");

    protected Lua Lua { get; }
    private Dictionary<string, ILuaModel> context { get; } = new();

    protected ScriptRunner()
    {
        Lua = new Lua();
        AddFunction("print", (string text) => Logger.Add($"[Script] {text}"));
        AddFunction("RandomRange", (int from, int to) => RNG.Next(from, to));
        Lua.DoString("import = function() end"); // disable importing
    }

    protected void AddFunction(string name, Delegate function)
    {
        Lua.RegisterFunction(name, function.Target, function.Method);
    }

    [CanBeNull]
    protected LuaFunction GetFunction(string name)
    {
        try
        {
            return Lua.GetFunction(name);
        }
        catch
        {
            return null;
        }
    }

    protected void AddContext(string name, ILuaModel value)
    {
        context[name] = value;
        Lua["ctx"] = context;
    }

    public void Run(string code)
    {
        Lua.DoString(code);
    }
}
