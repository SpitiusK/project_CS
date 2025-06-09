using System;
using System.Collections.Generic;

namespace func.brainfuck
{
    public class BrainfuckLoopCommands
    {
        public static void RegisterTo(IVirtualMachine vm)
        {
            var jumpMap = CreateJumpMap(vm.Instructions);
            vm.RegisterCommand('[', b =>
            {
                if (b.Memory[b.MemoryPointer] == 0)
                {
                    b.InstructionPointer = jumpMap[b.InstructionPointer];
                }
            });
            vm.RegisterCommand(']', b =>
            {
                if (b.Memory[b.MemoryPointer] != 0)
                {
                    b.InstructionPointer = jumpMap[b.InstructionPointer];
                }
            });
        }

        public static Dictionary<int, int> CreateJumpMap(string instructions)
        {
            var resultJumpMap = new Dictionary<int, int>();
            var bracketsStack = new Stack<int>();
            for (var i = 0; i < instructions.Length; i++)
            {
                if (instructions[i] == '[')
                {
                    bracketsStack.Push(i);
                }
                else if (instructions[i] == ']')
                {
                    if (bracketsStack.Count == 0)
                    {
                        throw new InvalidOperationException($"Unmatched ']' at position {i}");
                    }
                    var openPos = bracketsStack.Pop();
                    resultJumpMap[openPos] = i;
                    resultJumpMap[i] = openPos;
                }
            }
            if (bracketsStack.Count > 0)
            {
                throw new InvalidOperationException($"Unmatched '[' at position {bracketsStack.Peek()}");
            }
            return resultJumpMap;
        }
    }
}