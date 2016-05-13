using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		startX = maxW / 2;
		startY = maxH / 2;

		map = new int[maxW * maxH];
		for (int i = 0; i < maxW * maxH; i++)
			map [i] = 0;

		rnd = new Random ();

		generateMap (3);
		createTiles ();

		//GameObject obj = Instantiate (Resources.Load ("Tiles/forest-mod01")) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/////////

	public int maxW = 11;
	public int maxH = 11;
	public int pathLength = 3;
	private int endX, endY;

	private int startX;
	private int startY;
	private HashSet<KeyValuePair<int, int>> openedTiles;

	const int FLAG_RIGHT = 0;
	const int FLAG_LEFT = 1;
	const int FLAG_BOTTOM = 2;
	const int FLAG_TOP = 3;

	const int MASK_TOP = 1 << FLAG_TOP;
	const int MASK_BOTTOM = 1 << FLAG_BOTTOM;
	const int MASK_LEFT = 1 << FLAG_LEFT;
	const int MASK_RIGHT = 1 << FLAG_RIGHT;

	Random rnd;
	int [] map;

	/*MapGenerator::MapGenerator()
	{
		printf("MapGenerator::MapGenerator\n");
		map = new int[maxW * maxH];
		length = 3; //@note: input param
	}

	MapGenerator::~MapGenerator()
	{
		delete [] map;
	}*/

	private void printMap()
	{
		for(int j = 0; j < maxH; j++)
		{
			string line = "";
			for(int i = 0; i < maxW; i++)
			{
				int codeP = getPointerTo(i, j);
				line += map [codeP].ToString ("00");


			}
			Debug.Log (line);
		}
	}

	private int getRandomBool()
	{
		return Random.Range(0, 2); //@todo: check
	}

	private int getPointerTo(int x, int y)
	{
		if((x < 0) || (x >= maxW) || (y < 0) || (y >= maxH))
			return -1;

		return (y * maxW + x);
	}


	void generateCell(int x, int y)
	{
		int resultCodeP = getPointerTo(x, y);
		if(resultCodeP == -1)
			return;

		int prevCalculated = map[resultCodeP];

		int diffX = endX - x;
		int diffY = endY - y;

		int manhDist = Mathf.Abs(diffX) + Mathf.Abs(diffY);
		int resultPos = 0;

		if((x == startX) && (y == startY))
		{
			resultPos |= MASK_BOTTOM;
			resultPos |= MASK_TOP;
			resultPos |= MASK_LEFT;
			resultPos |= MASK_RIGHT;
		}
		else if(manhDist > pathLength * 2)
		{
			//@todo: do no generate pathes in direction other than end
			if(diffX > 0)
				resultPos |= getRandomBool() << FLAG_RIGHT;
			else if(diffX < 0)
				resultPos |= getRandomBool() << FLAG_LEFT;

			if(diffY > 0)
				resultPos |= getRandomBool() << FLAG_TOP;
			else if(diffY < 0)
				resultPos |= getRandomBool() << FLAG_BOTTOM;
		}
		else
		{
			//generate any random pathes
			resultPos |= getRandomBool() << FLAG_RIGHT;
			resultPos |= getRandomBool() << FLAG_LEFT;
			resultPos |= getRandomBool() << FLAG_TOP;
			resultPos |= getRandomBool() << FLAG_BOTTOM;
		}

		int unopenedPos = resultPos;

		//connect to neighbors
		/*std::function<void(int, int, int, int)> checkNeighbor = [&](int x, int y, int flag, int otherFlag)
		{


			int codeP = getPointerTo(x, y);
			if(codeP != -1)
			{
				int posCode = map[codeP];

				if(posCode != 0)
				{
					unopenedPos &= ~(1 << flag); //do not need to add to list this

					if(posCode & (1 << otherFlag) )
						resultPos |= (1 << flag);
					else
						resultPos &= ~(1 << flag);
				}
			}
			else
			{
				// memleak is bad idea
				resultPos &= ~(1 << flag);
				unopenedPos &= ~(1 << flag);
			}
		};*/

		checkNeighbor(x - 1, y, FLAG_LEFT, FLAG_RIGHT, ref resultPos, ref unopenedPos);
		checkNeighbor(x + 1, y, FLAG_RIGHT, FLAG_LEFT, ref resultPos, ref unopenedPos);
		checkNeighbor(x, y - 1, FLAG_BOTTOM, FLAG_TOP, ref resultPos, ref unopenedPos);
		checkNeighbor(x, y + 1, FLAG_TOP, FLAG_BOTTOM, ref resultPos, ref unopenedPos);

		//Fill up openedTiles
		if((unopenedPos & MASK_LEFT) > 0)
			openedTiles.Add(new KeyValuePair<int, int>(x - 1, y) );
		if((unopenedPos & MASK_RIGHT) > 0)
			openedTiles.Add(new KeyValuePair<int, int>(x + 1, y) );
		if((unopenedPos & MASK_TOP) > 0)
			openedTiles.Add(new KeyValuePair<int, int>(x, y + 1) );
		if((unopenedPos & MASK_BOTTOM) > 0)
			openedTiles.Add(new KeyValuePair<int, int>(x, y - 1) );

		map[resultCodeP] = (resultPos | prevCalculated);
	}

	private void checkNeighbor(int x, int y, int flag, int otherFlag, ref int resultPos, ref int unopenedPos)
	{
		int codeP = getPointerTo(x, y);
		if(codeP != -1)
		{
			int posCode = map[codeP];

			if(posCode != 0)
			{
				unopenedPos &= ~(1 << flag); //do not need to add to list this

				if((posCode & (1 << otherFlag) ) > 0)
					resultPos |= (1 << flag);
				else
					resultPos &= ~(1 << flag);
			}
		}
		else
		{
			// memleak is bad idea
			resultPos &= ~(1 << flag);
			unopenedPos &= ~(1 << flag);
		}
	}

	void generateMap(int length)
	{
		endX = Random.Range(0, length * 2 + 1) - length + startX;
		endY = Random.Range(0, length * 2 + 1) - length + startY;

		//openedTiles.clear();
		//openedTiles.insert(std::make_pair(startX, startY) );
		//memset(map, 0, sizeof(int) * (maxW * maxH));
		openedTiles = new HashSet<KeyValuePair<int, int>>();
		openedTiles.Add(new KeyValuePair<int, int>(startX, startY) );

		while (openedTiles.Count > 0)
		{
			foreach (KeyValuePair<int, int> pair in openedTiles) {
				openedTiles.Remove (pair);
				generateCell (pair.Key, pair.Value);
				break; // Shity solution, however, there is not simple iterator
			}

			//std::set<Point>::iterator pair = openedTiles.begin();
			//openedTiles.erase(pair);
			//generateCell(pair->first, pair->second);
		}
	}

	void createTiles()
	{
		for (int y = 0; y < maxH; y++) {
			for (int x = 0; x < maxW; x++) {
				int pointer = getPointerTo (x, y);
				int type = map [pointer];
				if(type > 0)
				{
					if ((type == 15) && (getRandomBool () == 0))
						type = 16;
						

					string fileName = "Tiles/forest-mod" + type.ToString("00");

					Debug.Log ("x: " + x + " y: " + y + " filename: " + fileName);
					GameObject obj = Instantiate (Resources.Load (fileName)) as GameObject;

					Vector3 newPos = new Vector3(x * 641, y * 450, 0);

					obj.transform.position = newPos;
					//obj.GetComponent<Transform>().position.Set (x * 1282, y * 900, 0);
				}

			}
		}

	}

}
