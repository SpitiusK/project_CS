using System;
using System.Collections;
using System.Collections.Generic;

namespace hashes
{
	public class ReadonlyBytes : IEnumerable<byte>
	{
		readonly byte[] _data;
		private readonly int _hashCode;
		public int Length => _data.Length;
		public ReadonlyBytes(params byte[] bytes)
		{
			_data = bytes ?? throw new ArgumentNullException("bytes");
			_hashCode = ComputeHashCode();
		}
		
		private int ComputeHashCode()
		{
			unchecked
			{
				uint hash = 2166136261;
				const uint prime = 16777619;

				foreach (byte b in _data)
				{
					hash = (hash ^ b) * prime;
				}
				return (int)hash;
			}
		}
		
		public byte this[int index]
		{
			get
			{
				if (index < 0 || index >= _data.Length)
				{
					throw new IndexOutOfRangeException();
				}
				return _data[index];
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(ReadonlyBytes)) return false;
			var other = (ReadonlyBytes)obj;
			if (other.Length != _data.Length) return false;
			for (var i = 0; i < _data.Length; i++)
			{
				if  (_data[i] != other[i]) return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}

		public override string ToString()
		{
			var resultString = "";
			foreach (var dataByte in _data)
			{
				resultString += $" {dataByte},";
			}
			if (resultString != "")
			{
				resultString = resultString.Remove(0, 1);
				resultString = resultString.Remove(resultString.Length - 1);
			}

			return new string($"[{resultString}]");
		}
		
		public IEnumerator<byte> GetEnumerator()
		{
			return ((IEnumerable<byte>)_data).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}