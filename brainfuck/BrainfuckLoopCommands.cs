using System.Collections.Generic;

namespace func.brainfuck
{
	public class BrainfuckLoopCommands
	{
		public static void RegisterTo(IVirtualMachine vm)
		{
			var isFirst = true;
			var jumpMap = new Dictionary<int, int>();
			vm.RegisterCommand('[', b => 
			{
				if (isFirst)
				{
					jumpMap = CreateJumpMap(b.Instructions);
					isFirst = false;
				}

				if (b.Memory[b.MemoryPointer] == 0)
				{
					b.InstructionPointer = jumpMap[b.InstructionPointer];
				}
			});
			vm.RegisterCommand(']', b =>
			{
				if (b.Memory[b.MemoryPointer] == 0) return;
				b.InstructionPointer = jumpMap[b.InstructionPointer];
			});
		}

		public static Dictionary<int,int> CreateJumpMap(string instructions)
		{
			var resultJumpMap = new Dictionary<int, int>();
			var bracketsStack = new Stack<(char, int)>();
			for (var i = 0; i < instructions.Length; i++)
			{
				if (instructions[i] == '[')
				{
					bracketsStack.Push((instructions[i], i));
					resultJumpMap[i] = 0; 
				}
				else if(instructions[i] == ']')
				{
					var currentBracket = bracketsStack.Pop();
					resultJumpMap[currentBracket.Item2] = i;
					resultJumpMap[i] = currentBracket.Item2;
				}
			}
			
			return resultJumpMap;
		}
	}
}