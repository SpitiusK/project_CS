using System;
using System.Text;

namespace hashes;

public class GhostsTask : 
	IFactory<Document>, IFactory<Vector>, IFactory<Segment>, IFactory<Cat>, IFactory<Robot>, 
	IMagic
{
	public bool IsNotVectorCreated = true;
	public Vector VectorCreated;
	
	public bool IsNotSegmentCreated = true;
	public Segment SegmentCreated;
	
	public bool IsNotCatCreated = true;
	public Cat CatCreated;
	
	public bool IsNotDocumentCreated = true;
	public byte[] Bytes;
	
	public bool IsNotRobotCreated = true;
	public void DoMagic()
	{
		if (!IsNotVectorCreated)
			VectorCreated.Add(new Vector(10, 10));

		if (!IsNotSegmentCreated)
		{
			SegmentCreated.Start.Add(new Vector(10, 10));
		}

		if (!IsNotCatCreated)
		{
			CatCreated.Rename("thisNotACat");
		}

		if (!IsNotDocumentCreated)
		{
			Bytes[0] = 1;
		}

		if (!IsNotRobotCreated)
		{
			Robot.BatteryCapacity = 0;
		}
	}

	// Чтобы класс одновременно реализовывал интерфейсы IFactory<A> и IFactory<B> 
	// придется воспользоваться так называемой явной реализацией интерфейса.
	// Чтобы отличать методы создания A и B у каждого метода Create нужно явно указать, к какому интерфейсу он относится.
	// На самом деле такое вы уже видели, когда реализовывали IEnumerable<T>.

	Vector IFactory<Vector>.Create()
	{
		var newVector = new Vector(1, 1);
		if (IsNotVectorCreated)
		{
			VectorCreated = newVector;
			IsNotVectorCreated = false;
		}
		return newVector;
	}

	Segment IFactory<Segment>.Create()
	{
		var newSegment = new Segment(new Vector(10, 10), new Vector(10, 10));
		if (IsNotSegmentCreated)
		{
			SegmentCreated = newSegment;
			IsNotSegmentCreated = false;
		}
			
		return newSegment;
	}

	// И так даллее по аналогии...
	
	Cat IFactory<Cat>.Create()
	{
		var newCat = new Cat("cat", "breed",  new DateTime(2000, 1, 1));

		if (IsNotCatCreated)
		{
			CatCreated = newCat;
			IsNotCatCreated = false;
		}
		return newCat;
	}
	
	Document IFactory<Document>.Create()
	{
		var newBytes = new byte[] { 10, 10, 10 };
		var newDocument = new Document("first", Encoding.UTF8, newBytes);
		if (IsNotDocumentCreated)
		{
			Bytes = newBytes;
			IsNotDocumentCreated = false;
		}
		return newDocument;
	}
	
	Robot IFactory<Robot>.Create()
	{
		var newRobot = new Robot("10");
		if (IsNotRobotCreated)
			IsNotRobotCreated = false;
		return newRobot;
	}
} 