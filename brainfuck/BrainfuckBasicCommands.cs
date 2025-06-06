using System;
using System.Collections.Generic;
using System.Linq;

namespace func.brainfuck;

public class BrainfuckBasicCommands
{
	public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
	{
		vm.RegisterCommand('+', b => PerformUncheckedOperation(b, vm, value => ++value));

		vm.RegisterCommand('-', b => PerformUncheckedOperation(b, vm, value => --value));

		vm.RegisterCommand('>', b => MoveMemoryPointer(b, 1));

		vm.RegisterCommand('<', b => MoveMemoryPointer(b, -1));

		vm.RegisterCommand(',', b => PerformUncheckedOperation(b, vm, _ => (byte)read()));


		RegisterAlphanumericCommands(vm);
	}
	
	private static byte GetCurrentMemoryValue(IVirtualMachine vm, IVirtualMachine brainfuck)
    {
        return brainfuck.Memory[vm.MemoryPointer];
    }
        
        
    private static void SetCurrentMemoryValue(IVirtualMachine vm, IVirtualMachine brainfuck, byte value)
    {
        brainfuck.Memory[vm.MemoryPointer] = value;
    }
        
        
    private static void PerformUncheckedOperation(IVirtualMachine vm, IVirtualMachine brainfuck, Func<byte, byte> operation)
    {
        unchecked
        {
            byte currentValue = GetCurrentMemoryValue(vm, brainfuck);
            byte newValue = operation(currentValue);
            SetCurrentMemoryValue(vm, brainfuck, newValue);
        }
    }
        
        
    private static void MoveMemoryPointer(IVirtualMachine brainfuck, int direction)
    {
        int newPointer = brainfuck.MemoryPointer + direction;
        if (newPointer >= brainfuck.Memory.Length)
        {
            brainfuck.MemoryPointer = 0;
        }
        else if (newPointer < 0)
        {
            brainfuck.MemoryPointer = brainfuck.Memory.Length - 1;
        }
        else
        {
            brainfuck.MemoryPointer = newPointer;
        }
    }
        
        
    private static void RegisterAlphanumericCommands(IVirtualMachine vm)
    {
     
        var symbols = Enumerable.Range('A', 26) 
            .Concat(Enumerable.Range('a', 26)) 
            .Concat(Enumerable.Range('0', 10)) 
            .Select(c => (char)c);

            foreach (char symbol in symbols)
            {
                vm.RegisterCommand(symbol, b => SetCurrentMemoryValue(vm, b, (byte)symbol));
            }
    }
}
    


