using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConsoleCommand
{
    public string Name { get; }
    public string Description { get; }
    public Action<string[]> Execute { get; }

    public ConsoleCommand(string name, string description, Action<string[]> execute)
    {
        Name = name.ToLower();
        Description = description;
        Execute = execute;
    }
}

public static class ConsoleExtensions
{
    public static Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();

    static ConsoleExtensions()
    {
        Initialize();
    }

    static void Initialize()
    {
        RegisterCommand(new ConsoleCommand("help", "Shows all available commands", args =>
        {
            foreach (var cmd in commands.Values)
            {
                AddLog($"{cmd.Name} - {cmd.Description}");
                Debug.Log($"{cmd.Name} - {cmd.Description}");
            }
        }));

        RegisterCommand(new ConsoleCommand("clear", "Clears the console output", args =>
        {
            Debug.Log("Console cleared.");
            ConsoleManager.instance.logContainer.Clear();
            AddLog("Console cleared.", Color.green);
            // Tu peux aussi vider l’historique visuel ici si tu en as un
        }));

        RegisterCommand(new ConsoleCommand("exit", "Closes the game", args =>
        {
            Debug.Log("Closing the game...");
            AddLog("Closing the game...", Color.green);
            Application.Quit();
        }));

        RegisterCommand(new ConsoleCommand("tp", "Teleports player to coordinates", args =>
        {
            if (args.Length < 3) { Debug.Log("Usage: tp x y z"); return; }

            if (float.TryParse(args[0], out var x) &&
                float.TryParse(args[1], out var y) &&
                float.TryParse(args[2], out var z))
            {
                // Find the player object by tag (ensure your player has the "Player" tag)
                var player = GameObject.FindWithTag("Player");
                if (player != null) player.transform.position = new Vector3(x, y, z);

                // Move the camera as well if needed
                var cam = Camera.main;
                if (cam != null) cam.transform.position = new Vector3(x, y + 0.5f, z);

                AddLog($"Teleported to {x}, {y}, {z}", Color.green);
                Debug.Log($"Teleported to {x}, {y}, {z}");
            }
            else
            {
                AddLog("Invalid coordinates.", Color.orange);
                Debug.Log("Invalid coordinates.");
            }
        }));

        RegisterCommand(new ConsoleCommand("get_pos", "Gets player current position", args =>
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var pos = player.transform.position;
                AddLog($"Player position: {pos.x}, {pos.y}, {pos.z}", Color.cyan);
                Debug.Log($"Player position: {pos.x}, {pos.y}, {pos.z}");
            }
            else
            {
                AddLog("Player object not found.", Color.orange);
                Debug.Log("Player object not found.");
            }
        }));

        RegisterCommand(new ConsoleCommand("player", "Contains player related commands", args =>
        {
            // registre interne aux sous-commandes player
            var playerSubCommands = new Dictionary<string, Action<string[]>>()
            {
                ["set_speed"] = subArgs =>
                {
                    if (subArgs.Length < 1)
                    {
                        AddLog($"Usage: player set_speed <value>. Current: {ConsoleManager.instance.playerCtrl.speed}", Color.orange);
                        return;
                    }

                    if (float.TryParse(subArgs[0], out var speed))
                    {
                        if (ConsoleManager.instance.playerCtrl != null)
                        {
                            ConsoleManager.instance.playerCtrl.speed = speed;
                            AddLog($"Player speed set to {speed}", Color.green);
                        }
                        else
                            AddLog("CharacterMovement reference is not assigned in ConsoleManager.", Color.red);
                    }
                    else AddLog("Invalid speed value.", Color.red);
                },

                ["set_jump"] = subArgs =>
                {
                    if (subArgs.Length < 1)
                    {
                        AddLog("Usage: player set_jump <value>", Color.orange);
                        return;
                    }

                    if (float.TryParse(subArgs[0], out var jumpForce))
                    {
                        if (ConsoleManager.instance.playerCtrl != null)
                        {
                            ConsoleManager.instance.playerCtrl.jumpForce = jumpForce;
                            AddLog($"Player jump force set to {jumpForce}", Color.green);
                        }
                        else
                            AddLog("CharacterMovement reference is not assigned in ConsoleManager.", Color.red);
                    }
                    else AddLog("Invalid jump force value.", Color.red);
                }
            };

            // gestion des arguments
            if (args.Length == 0)
            {
                AddLog("Usage: player <subCommand> [args]", Color.orange);
                AddLog("Available sub-commands: " + string.Join(", ", playerSubCommands.Keys), Color.cyan);
                return;
            }

            var subCommand = args[0].ToLower();
            var subArgs = args.Length > 1 ? args[1..] : Array.Empty<string>();

            if (playerSubCommands.TryGetValue(subCommand, out var action))
            {
                AddLog($"-- Executed player sub-command: {subCommand} --", Color.green);
                action.Invoke(subArgs);
            }
            else
            {
                AddLog($"Unknown player sub-command: {subCommand}. Available: {string.Join(", ", playerSubCommands.Keys)}", Color.red);
            }
        }));

        RegisterCommand(new ConsoleCommand("set_sensitivity", "Sets camera sensitivity", args =>
        {
            if (args.Length < 1) { Debug.Log("Usage: set_sensitivity value"); return; }
            if (float.TryParse(args[0], out var sensitivity))
            {
                if (ConsoleManager.instance.cameraCtrl != null)
                {
                    ConsoleManager.instance.cameraCtrl.sensitivity = sensitivity;
                    AddLog($"Camera sensitivity set to {sensitivity}", Color.green);
                    Debug.Log($"Camera sensitivity set to {sensitivity}");
                }
                else
                {
                    AddLog("CameraMovement reference is not assigned in ConsoleManager.", Color.orange);
                    Debug.LogWarning("CameraMovement reference is not assigned in ConsoleManager.");
                }
            }
            else
            {
                AddLog("Invalid sensitivity value.", Color.orange);
                Debug.Log("Invalid sensitivity value.");
            }
        }));

        RegisterCommand(new ConsoleCommand("spawn", "Spawns an object by prefab name to the position defined", args =>
        {
            if (args.Length < 1) { AddLog("Usage: spawn prefabName"); return; }

            string prefabName = args[0];
            GameObject prefab = Resources.Load<GameObject>(prefabName);

            if (prefab != null)
            {
                Vector3 spawnPos = Vector3.zero;
                if (args.Length >= 4 &&
                    float.TryParse(args[1], out float x) &&
                    float.TryParse(args[2], out float y) &&
                    float.TryParse(args[3], out float z))
                {
                    spawnPos = new Vector3(x, y, z);
                }
                else
                {
                    // spawn devant le joueur par défaut
                    var player = GameObject.FindWithTag("Player");
                    if (player != null)
                        spawnPos = player.transform.position + player.transform.forward * 2f;
                }

                GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
                AddLog($"Spawned {prefabName} at {spawnPos}");
            }
            else
            {
                AddLog($"Prefab '{prefabName}' not found in Resources.");
            }
        }));
    }

    public static void RegisterCommand(ConsoleCommand command)
    {
        if (!commands.ContainsKey(command.Name))
            commands.Add(command.Name, command);
        else
        {
            Debug.LogWarning($"Command {command.Name} already exists.");
            AddLog($"Command {command.Name} already exists.", Color.orange);
        }
    }

    public static void ProcessCommands(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        var splitArgs = input.Split(' ');
        var commandName = splitArgs[0].ToLower();
        var args = splitArgs.Length > 1 ? splitArgs[1..] : new string[0];

        if (commands.TryGetValue(commandName, out var command))
        {
            AddLog($"-- Executed command: {command} --", Color.green);
            command.Execute.Invoke(args);
        }
        else
        {
            Debug.Log($"Unknown command: {commandName}. Type 'help' to list all commands.");
            AddLog($"Unknown command: {commandName}. Type 'help' to list all commands.", Color.red);
        }
    }

    public static void AddLog(string message, Color color = default)
    {
        if (color == default) color = Color.white;

        Label logEntry = new Label(message);
        logEntry.style.whiteSpace = WhiteSpace.Normal;
        logEntry.style.marginBottom = 2;
        logEntry.style.color = color;

        ConsoleManager.instance.logContainer.Add(logEntry);
        ConsoleManager.instance.logContainer.ScrollTo(logEntry);
    }
}