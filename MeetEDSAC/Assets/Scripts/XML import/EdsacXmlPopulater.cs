using UnityEngine;
using System.Collections;

public class EdsacXmlPopulater : MonoBehaviour {

	public TextAsset edsacXml;
	public EDSAC edsac;
	private TinyXmlReader reader;

	// Use this for initialization
	public EDSAC Populate() {
		edsac = new EDSAC();
		reader = new TinyXmlReader(edsacXml.text);
		string readingDebugText = "Debug output from XML population:\n\n";
		while (reader.Read()) {
			if (reader.isOpeningTag) {
				if (reader.tagName == "chassisdefinition") {
					readingDebugText += "New chassis definition found.\n";
					EDSAC.ChassisType c = new EDSAC.ChassisType();
					string key = "";
					while (reader.tagName != "chassisdefinition" || reader.isOpeningTag) {
						reader.Read();
						if (reader.isOpeningTag) {
							switch(reader.tagName) {
							case "chassistype":
								key = reader.content;
								readingDebugText += "Chassis definition key: " + key + "\n";
								break;
							case "testingpoints":
								switch (reader.content) {
								case "1":
									c.testingPoints = EDSAC.TestingPointsType.TYPE1;
									break;
								case "2":
									c.testingPoints = EDSAC.TestingPointsType.TYPE2;
									break;
								case "3":
									c.testingPoints = EDSAC.TestingPointsType.TYPE3;
									break;
								case "4":
									c.testingPoints = EDSAC.TestingPointsType.TYPE4;
									break;
								}
								readingDebugText += "Chassis testing points: " + c.testingPoints.ToString() + "\n";
								break;
							case "valve":
								EDSAC.Valve v = new EDSAC.Valve();
								readingDebugText += "New valve.\n";
								while (reader.tagName != "valve" || reader.isOpeningTag) {
									reader.Read();
									if (reader.isOpeningTag) {
										switch(reader.tagName) {
										case "valvetype":
											switch (reader.content) {
											case "SmallRed":
												v.valveType = EDSAC.ValveType.SMALL_RED;
												break;
											case "SmallCream":
												v.valveType = EDSAC.ValveType.SMALL_CREAM;
												break;
											case "SmallBlack":
												v.valveType = EDSAC.ValveType.SMALL_BLACK;
												break;
											case "BigFlat":
												v.valveType = EDSAC.ValveType.BIG_FLAT;
												break;
											case "BigShiny":
												v.valveType = EDSAC.ValveType.BIG_SHINY;
												break;
											}
											readingDebugText += "Valve type: " + v.valveType.ToString() + "\n";
											break;
										case "valvex":
											v.x = float.Parse(reader.content);
											readingDebugText += "Valve x: " + v.x + "\n";
											break;
										case "valvey":
											v.y = float.Parse(reader.content);
											readingDebugText += "Valve y: " + v.y + "\n";
											break;
										}
									}
								}
								c.valves.Add (v);
								break;
							}
						}
					}
					edsac.availableChassisTypes.Add(key, c);
				} else if (reader.tagName == "row") {
					readingDebugText += "New row found.\n";
					EDSAC.RowName rowName = EDSAC.RowName.UNKNOWN;
					while (reader.tagName != "row" || reader.isOpeningTag) {
						reader.Read();
						if (reader.isOpeningTag) {
							switch(reader.tagName) {
							case "rowname":
								switch(reader.content) {
								case "Rear":
									rowName = EDSAC.RowName.REAR;
									break;
								case "Middle":
									rowName = EDSAC.RowName.MIDDLE;
									break;
								case "Front":
									rowName = EDSAC.RowName.FRONT;
									break;
								}
								readingDebugText += "Row type: " + rowName.ToString() + "\n";
								break;
							case "rack":
								EDSAC.Rack r = new EDSAC.Rack();
								readingDebugText += "New rack found.\n";
								while (reader.tagName != "rack" || reader.isOpeningTag) {
									reader.Read();
									if (reader.isOpeningTag) {
										switch(reader.tagName) {
										case "racknumber":
											r.rackNumber = int.Parse(reader.content);
											readingDebugText += "Rack number: " + r.rackNumber + "\n";
											break;
										case "chassis":
											readingDebugText += "New chassis found.\n";
											EDSAC.Chassis c = new EDSAC.Chassis();
											while (reader.tagName != "chassis" || reader.isOpeningTag) {
												reader.Read();
												if (reader.isOpeningTag) {
													switch(reader.tagName) {
													case "chassisnumber":
														c.chassisNumber = int.Parse(reader.content);
														readingDebugText += "Chassis number: " + c.chassisNumber + "\n";
														break;
													case "chassisworldlabeltext":
														c.physicalLabelNameText = reader.content;
														readingDebugText += "Chassis label text: " + c.physicalLabelNameText + "\n";
														break;
													case "chassisworldlabelnumber":
														c.physicalLabelNumberText = reader.content;
														readingDebugText += "Chassis label number: " + c.physicalLabelNumberText + "\n";
														break;
													case "chassisuilabel":
														c.uiLabelText = reader.content;
														readingDebugText += "Chassis UI label: " + c.uiLabelText + "\n";
														break;
													case "chassistype":
														if (edsac.availableChassisTypes.ContainsKey(reader.content)) {
															c.chassisType = edsac.availableChassisTypes[reader.content];
														} else {
															c.chassisType = new EDSAC.ChassisType();
														}
														readingDebugText += "Chassis type: " + reader.content + "\n";
														break;
													}
												}
											}
											r.chassis.Add(c);
											break;
										}
									}
								}
								edsac.rows[rowName].racks.Add(r);
								break;
							}
						}
					}
				}
			}
		}
		Debug.Log (readingDebugText);
		return edsac;
	}

}
