using System;

namespace func.brainfuck
{
    public static class BrainfuckBasicCommands
    {
        private const string AlphanumericChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";

        public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
        {
            RegisterBasicCommands(vm, read, write);
            RegisterAlphanumericCommands(vm);
        }

        private static void RegisterBasicCommands(IVirtualMachine vm, Func<int> read, Action<char> write)
        {
            vm.RegisterCommand('.', (IVirtualMachine v) => write((char)v.Memory[v.MemoryPointer]));
            vm.RegisterCommand('+', (IVirtualMachine v) => AdjustMemory(v, 1));
            vm.RegisterCommand('-', (IVirtualMachine v) => AdjustMemory(v, -1));
            vm.RegisterCommand('>', (IVirtualMachine v) => MovePointer(v, 1));
            vm.RegisterCommand('<', (IVirtualMachine v) => MovePointer(v, -1));
            vm.RegisterCommand(',', (IVirtualMachine v) => v.Memory[v.MemoryPointer] = (byte)read());
        }

        private static void RegisterAlphanumericCommands(IVirtualMachine vm)
        {
            foreach (char c in AlphanumericChars)
            {
                vm.RegisterCommand(c, (IVirtualMachine v) => v.Memory[v.MemoryPointer] = (byte)c);
            }
        }

        private static void AdjustMemory(IVirtualMachine vm, int amount)
        {
            vm.Memory[vm.MemoryPointer] = unchecked((byte)(vm.Memory[vm.MemoryPointer] + amount));
        }

        private static void MovePointer(IVirtualMachine vm, int direction)
        {
            int newPointer = vm.MemoryPointer + direction;
            vm.MemoryPointer = (newPointer + vm.Memory.Length) % vm.Memory.Length;
        }
    }
}