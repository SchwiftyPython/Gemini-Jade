// Disable some warnings since this class compiles out large parts of the code depending on compiler directives
#pragma warning disable 0162
#pragma warning disable 0414
#pragma warning disable 0429
//#define PROFILE // Uncomment to enable profiling
//#define KEEP_SAMPLES
using System;
using System.Collections.Generic;

namespace Pathfinding {
	/// <summary>
	/// The profile class
	/// </summary>
	public class Profile {
		/// <summary>
		/// The profile mem
		/// </summary>
		const bool PROFILE_MEM = false;

		/// <summary>
		/// The name
		/// </summary>
		public readonly string name;
		/// <summary>
		/// The watch
		/// </summary>
		readonly System.Diagnostics.Stopwatch watch;
		/// <summary>
		/// The counter
		/// </summary>
		int counter;
		/// <summary>
		/// The mem
		/// </summary>
		long mem;
		/// <summary>
		/// The smem
		/// </summary>
		long smem;

#if KEEP_SAMPLES
		List<float> samples = new List<float>();
#endif

		/// <summary>
		/// The control
		/// </summary>
		int control = 1 << 30;
		/// <summary>
		/// The dont count first
		/// </summary>
		const bool dontCountFirst = false;

		/// <summary>
		/// Controls the value
		/// </summary>
		/// <returns>The control</returns>
		public int ControlValue () {
			return control;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Profile"/> class
		/// </summary>
		/// <param name="name">The name</param>
		public Profile (string name) {
			this.name = name;
			watch = new System.Diagnostics.Stopwatch();
		}

		/// <summary>
		/// Writes the csv using the specified path
		/// </summary>
		/// <param name="path">The path</param>
		/// <param name="profiles">The profiles</param>
		public static void WriteCSV (string path, params Profile[] profiles) {
#if KEEP_SAMPLES
			var s = new System.Text.StringBuilder();
			s.AppendLine("x, y");
			foreach (var profile in profiles) {
				for (int i = 0; i < profile.samples.Count; i++) {
					s.AppendLine(profile.name + ", " + profile.samples[i].ToString("R"));
				}
			}
			System.IO.File.WriteAllText(path, s.ToString());
#endif
		}

		/// <summary>
		/// Runs the action
		/// </summary>
		/// <param name="action">The action</param>
		public void Run (System.Action action) {
			Start();
			action();
			Stop();
		}

		/// <summary>
		/// Starts this instance
		/// </summary>
		[System.Diagnostics.ConditionalAttribute("PROFILE")]
		public void Start () {
			if (PROFILE_MEM) {
				smem = GC.GetTotalMemory(false);
			}
			if (dontCountFirst && counter == 1) return;
			watch.Start();
		}

		/// <summary>
		/// Stops this instance
		/// </summary>
		[System.Diagnostics.ConditionalAttribute("PROFILE")]
		public void Stop () {
			counter++;
			if (dontCountFirst && counter == 1) return;

			watch.Stop();
			if (PROFILE_MEM) {
				mem += GC.GetTotalMemory(false)-smem;
			}
#if KEEP_SAMPLES
			samples.Add((float)watch.Elapsed.TotalMilliseconds);
			watch.Reset();
#endif
		}

		/// <summary>
		/// Logs this instance
		/// </summary>
		[System.Diagnostics.ConditionalAttribute("PROFILE")]
		/// <summary>Log using Debug.Log</summary>
		public void Log () {
			UnityEngine.Debug.Log(ToString());
		}

		/// <summary>
		/// Consoles the log
		/// </summary>
		[System.Diagnostics.ConditionalAttribute("PROFILE")]
		/// <summary>Log using System.Console</summary>
		public void ConsoleLog () {
#if !NETFX_CORE || UNITY_EDITOR
			System.Console.WriteLine(ToString());
#endif
		}

		/// <summary>
		/// Stops the control
		/// </summary>
		/// <param name="control">The control</param>
		/// <exception cref="Exception"></exception>
		[System.Diagnostics.ConditionalAttribute("PROFILE")]
		public void Stop (int control) {
			counter++;
			if (dontCountFirst && counter == 1) return;

			watch.Stop();
			if (PROFILE_MEM) {
				mem += GC.GetTotalMemory(false)-smem;
			}

			if (this.control == 1 << 30) this.control = control;
			else if (this.control != control) throw new Exception("Control numbers do not match " + this.control + " != " + control);
		}

		/// <summary>
		/// Controls the other
		/// </summary>
		/// <param name="other">The other</param>
		/// <exception cref="Exception"></exception>
		[System.Diagnostics.ConditionalAttribute("PROFILE")]
		public void Control (Profile other) {
			if (ControlValue() != other.ControlValue()) {
				throw new Exception("Control numbers do not match ("+name + " " + other.name + ") " + ControlValue() + " != " + other.ControlValue());
			}
		}

		/// <summary>
		/// Returns the string
		/// </summary>
		/// <returns>The </returns>
		public override string ToString () {
			string s = name + " #" + counter + " " + watch.Elapsed.TotalMilliseconds.ToString("0.0 ms") + " avg: " + (watch.Elapsed.TotalMilliseconds/counter).ToString("0.00 ms");

			if (PROFILE_MEM) {
				s += " avg mem: " + (mem/(1.0*counter)).ToString("0 bytes");
			}
			return s;
		}
	}
}
