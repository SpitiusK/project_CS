

using Avalonia.Input;
using Digger.Architecture;

namespace Digger;


public static class Game
{
	private const string mapWithPlayerTerrain = @"
TTT T
TTP T
T T T
TT TT";

	private const string mapWithPlayerTerrainSackGold = @"
PTTGTT TS
TST  TSTT
TTTTTTSTT
T TSTS TT
T TTTG ST
TSTSTT TT";

	private const string mapWithPlayerTerrainSackGoldMonster = @"
PTTGTTSTSS
TST  TMTTM
TTT TT TTT
T TSTS TTT
T TTTT STT
T T TT  TT
TSTSTT TTT
S TTST TTG
 TGST  TTT
 T  T TTTT";

	public static ICreature[,] Map;
	public static int Scores;
	public static bool IsOver;

	public static Key KeyPressed;
	public static int MapWidth => Map.GetLength(0);
	public static int MapHeight => Map.GetLength(1);

	public static void CreateMap()
	{
		Map = CreatureMapCreator.CreateMap(mapWithPlayerTerrainSackGoldMonster);
	}
}