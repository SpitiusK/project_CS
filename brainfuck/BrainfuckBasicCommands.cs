using System;
using System.Collections.Generic;
using System.Linq;

namespace func.brainfuck
{
	public class BrainfuckBasicCommands
	{
		public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
		{
			vm.RegisterCommand('.', b => { write((char)b.Memory[vm.MemoryPointer]);});
			vm.RegisterCommand('+', b => {
				unchecked
				{
					b.Memory[vm.MemoryPointer]++;	
				}
			}); 
			vm.RegisterCommand('-', b => {
				unchecked
				{
					vm.Memory[b.MemoryPointer]--;
				}
			});
			vm.RegisterCommand('>', b =>
			{
				if (b.MemoryPointer + 1 > b.Memory.Length - 1)
				{
					b.MemoryPointer = 0; 
					return;
				}
				b.MemoryPointer++;
			});
			
			vm.RegisterCommand('<', b =>
			{
				if (b.MemoryPointer - 1 < 0)
				{
					b.MemoryPointer = b.Memory.Length - 1;
					return;
				}
				b.MemoryPointer--;
			});
			
			vm.RegisterCommand(',', b =>
			{
				unchecked
				{
					var newReadValue = (byte)read();
					b.Memory[vm.MemoryPointer] = newReadValue;
				}
			});

			foreach (var symbol in "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890")
			{
				vm.RegisterCommand(symbol, b =>
				{
					b.Memory[vm.MemoryPointer] = (byte)symbol;
				});
			}
		}
	}
}