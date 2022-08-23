namespace Pathfinding.Util {
	/// <summary>
	/// Simple implementation of a GUID.
	/// Version: Since 3.6.4 this struct works properly on platforms with different endianness such as Wii U.
	/// </summary>
	public struct Guid {
		/// <summary>
		/// The hex
		/// </summary>
		const string hex = "0123456789ABCDEF";

		/// <summary>
		/// The guid
		/// </summary>
		public static readonly Guid zero = new Guid(new byte[16]);
		/// <summary>
		/// The to string
		/// </summary>
		public static readonly string zeroString = new Guid(new byte[16]).ToString();

		/// <summary>
		/// The 
		/// </summary>
		readonly ulong _a, _b;

		/// <summary>
		/// Initializes a new instance of the <see cref="Guid"/> class
		/// </summary>
		/// <param name="bytes">The bytes</param>
		public Guid (byte[] bytes) {
			// Pack 128 bits into 2 longs
			ulong a = ((ulong)bytes[0] << 8*0) |
					  ((ulong)bytes[1] << 8*1) |
					  ((ulong)bytes[2] << 8*2) |
					  ((ulong)bytes[3] << 8*3) |
					  ((ulong)bytes[4] << 8*4) |
					  ((ulong)bytes[5] << 8*5) |
					  ((ulong)bytes[6] << 8*6) |
					  ((ulong)bytes[7] << 8*7);

			ulong b = ((ulong)bytes[8] <<  8*0) |
					  ((ulong)bytes[9] <<  8*1) |
					  ((ulong)bytes[10] << 8*2) |
					  ((ulong)bytes[11] << 8*3) |
					  ((ulong)bytes[12] << 8*4) |
					  ((ulong)bytes[13] << 8*5) |
					  ((ulong)bytes[14] << 8*6) |
					  ((ulong)bytes[15] << 8*7);

			// Need to swap endianness on e.g Wii U
			_a = System.BitConverter.IsLittleEndian ? a : SwapEndianness(a);
			_b = System.BitConverter.IsLittleEndian ? b : SwapEndianness(b);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Guid"/> class
		/// </summary>
		/// <param name="str">The str</param>
		/// <exception cref="System.FormatException"></exception>
		/// <exception cref="System.FormatException"></exception>
		/// <exception cref="System.FormatException">Invalid Guid format</exception>
		/// <exception cref="System.FormatException">Invalid Guid format. String too short</exception>
		/// <exception cref="System.FormatException">Invalid Guid format. String too short</exception>
		public Guid (string str) {
			_a = 0;
			_b = 0;

			if (str.Length < 32)
				throw new System.FormatException("Invalid Guid format");

			int counter = 0;
			int i = 0;
			int offset = 15*4;

			for (; counter < 16; i++) {
				if (i >= str.Length)
					throw new System.FormatException("Invalid Guid format. String too short");

				char c = str[i];
				if (c == '-') continue;

				//Neat trick, perhaps a bit slow, but one will probably not use Guid parsing that much
				int value = hex.IndexOf(char.ToUpperInvariant(c));
				if (value == -1)
					throw new System.FormatException("Invalid Guid format : "+c+" is not a hexadecimal character");

				_a |= (ulong)value << offset;
				//SetByte (counter,(byte)value);
				offset -= 4;
				counter++;
			}

			offset = 15*4;
			for (; counter < 32; i++) {
				if (i >= str.Length)
					throw new System.FormatException("Invalid Guid format. String too short");

				char c = str[i];
				if (c == '-') continue;

				//Neat trick, perhaps a bit slow, but one will probably not use Guid parsing that much
				int value = hex.IndexOf(char.ToUpperInvariant(c));
				if (value == -1)
					throw new System.FormatException("Invalid Guid format : "+c+" is not a hexadecimal character");

				_b |= (ulong)value << offset;
				//SetByte (counter,(byte)value);
				offset -= 4;
				counter++;
			}
		}

		/// <summary>
		/// Parses the input
		/// </summary>
		/// <param name="input">The input</param>
		/// <returns>The guid</returns>
		public static Guid Parse (string input) {
			return new Guid(input);
		}

		/// <summary>Swaps between little and big endian</summary>
		static ulong SwapEndianness (ulong value) {
			var b1 = (value >> 0) & 0xff;
			var b2 = (value >> 8) & 0xff;
			var b3 = (value >> 16) & 0xff;
			var b4 = (value >> 24) & 0xff;
			var b5 = (value >> 32) & 0xff;
			var b6 = (value >> 40) & 0xff;
			var b7 = (value >> 48) & 0xff;
			var b8 = (value >> 56) & 0xff;

			return b1 << 56 | b2 << 48 | b3 << 40 | b4 << 32 | b5 << 24 | b6 << 16 | b7 << 8 | b8 << 0;
		}

		/// <summary>
		/// Returns the byte array
		/// </summary>
		/// <returns>The bytes</returns>
		public byte[] ToByteArray () {
			var bytes = new byte[16];

			byte[] ba = System.BitConverter.GetBytes(!System.BitConverter.IsLittleEndian ? SwapEndianness(_a) : _a);
			byte[] bb = System.BitConverter.GetBytes(!System.BitConverter.IsLittleEndian ? SwapEndianness(_b) : _b);

			for (int i = 0; i < 8; i++) {
				bytes[i] = ba[i];
				bytes[i+8] = bb[i];
			}
			return bytes;
		}

		/// <summary>
		/// The random
		/// </summary>
		private static System.Random random = new System.Random();

		/// <summary>
		/// News the guid
		/// </summary>
		/// <returns>The guid</returns>
		public static Guid NewGuid () {
			var bytes = new byte[16];

			random.NextBytes(bytes);
			return new Guid(bytes);
		}

		public static bool operator == (Guid lhs, Guid rhs) {
			return lhs._a == rhs._a && lhs._b == rhs._b;
		}

		public static bool operator != (Guid lhs, Guid rhs) {
			return lhs._a != rhs._a || lhs._b != rhs._b;
		}

		/// <summary>
		/// Describes whether this instance equals
		/// </summary>
		/// <param name="_rhs">The rhs</param>
		/// <returns>The bool</returns>
		public override bool Equals (System.Object _rhs) {
			if (!(_rhs is Guid)) return false;

			var rhs = (Guid)_rhs;

			return _a == rhs._a && _b == rhs._b;
		}

		/// <summary>
		/// Gets the hash code
		/// </summary>
		/// <returns>The int</returns>
		public override int GetHashCode () {
			ulong ab = _a ^ _b;

			return (int)(ab >> 32) ^ (int)ab;
		}

		/// <summary>
		/// The text
		/// </summary>
		private static System.Text.StringBuilder text;

		/// <summary>
		/// Returns the string
		/// </summary>
		/// <returns>The string</returns>
		public override string ToString () {
			if (text == null) {
				text = new System.Text.StringBuilder();
			}
			lock (text) {
				text.Length = 0;
				text.Append(_a.ToString("x16")).Append('-').Append(_b.ToString("x16"));
				return text.ToString();
			}
		}
	}
}
