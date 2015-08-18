using System.Collections.Generic;

public class EDSAC {
	
	public enum RowName { UNKNOWN, REAR, MIDDLE, FRONT };
	
	public enum ValveType { UNKNOWN, SMALL_RED, SMALL_CREAM, SMALL_BLACK, BIG_FLAT, BIG_SHINY };
	
	public enum TestingPointsType { TYPE1, TYPE2, TYPE3, TYPE4 };

	#region class-definitions
	public class Row {

		public RowName name;
		public IList<Rack> racks;

		public Row() {
			racks = new List<Rack>();
		}

	}

	public class Rack {

		public IList<Chassis> chassis;
		public int rackNumber;
		
		public Rack() {
			chassis = new List<Chassis>();
		}

	}

	public class Chassis {

		public ChassisType chassisType;
		public int chassisNumber; // position within the rack
		public string physicalLabelNameText; // e.g. "MEMOMRY UNIT"
		public string physicalLabelNumberText; // e.g. "1/5"
		public string uiLabelText; // e.g. "Storage Regeneration Unit"

	}
	
	public class Valve {
		
		public ValveType valveType;
		public float x;
		public float y;

		public Valve() {
			valveType = ValveType.UNKNOWN;
		}
		
	}

	public class ChassisType {

		public IList<Valve> valves;
		public TestingPointsType testingPoints;
		
		public ChassisType() {
			valves = new List<Valve>();
		}

	}
	#endregion

	public Dictionary<string,ChassisType> availableChassisTypes;

	public Dictionary<RowName,Row> rows;

	public EDSAC() {

		availableChassisTypes = new Dictionary<string, ChassisType>();
		rows = new Dictionary<RowName, Row>();
		rows.Add (RowName.REAR, new Row());
		rows.Add (RowName.MIDDLE, new Row());
		rows.Add (RowName.FRONT, new Row());
		rows[RowName.REAR].name = RowName.REAR;
		rows[RowName.MIDDLE].name = RowName.MIDDLE;
		rows[RowName.FRONT].name = RowName.FRONT;

	}

}
