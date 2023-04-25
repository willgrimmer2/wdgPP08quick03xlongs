#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
//using NinjaTrader.NinjaScript.Extensions;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class wdgPP08quick03xlongs : Strategy
	{
		private EMA EMA1;
		private EMA EMA2;
		private GrimmerLegVol04 glv;
		
		private double myStop;
		private double myStopl;
		private double myStops;
		private double finalTarget;
		private double finalTargetl;
		private double finalTargets;
		private double breakevenTarget;
		private double breakevenTargetl;
		private double breakevenTargets;
		private double entryPrice;
		
		//private GrimmerLegVol grimmerLegVol;
		
		private bool myFreeTrade;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "wdgPP08quick03xlongs";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				Target					= 2;
				Stop					= 1;
				Cars 					= 2;
				Tix 					= 64;
				Tixperpoint 			= 4;
				
			{
        // Add GrimmerLegVol04 indicator to the chart
        //AddChartIndicator<GrimmerLegVol04>();
				AddChartIndicator(new GrimmerLegVol04());
    }
				
			}
			else if (State == State.Configure)
			{
				//myStop 				= Low[2] - (Stop * TickSize);
			}
			else if (State == State.DataLoaded)
			{	
				ClearOutputWindow(); 
				
				EMA1				= EMA(Close, 8);
				EMA2				= EMA(Close, 21);
				//myStop 				= Low[2] - (Stop * TickSize);
				
				// Create an instance of the GrimmerLegVol04 indicator
        		glv = GrimmerLegVol04();
        		AddChartIndicator(glv);
				
				//SetProfitTarget("", CalculationMode.Price, Position.AveragePrice + ( Position.AveragePrice  - Low[BarsSinceEntryExecution()+1] ));
				//SetStopLoss("", CalculationMode.Price, Low[BarsSinceEntryExecution()+3], false);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 7)
				return;
			
			//GrimmerLegVol04 glv = Indicators.ToIndicator<GrimmerLegVol04>();
			//var glv = ToIndicator<GrimmerLegVol04>();
			
			// Define and initialize the GrimmerLegVol indicator
    		//GrimmerLegVol04 glv = NinjaTrader.NinjaScript.Core.NinjaScriptBase.GetIndicator<GrimmerLegVol04>();

			 // Set 1 - Go Long
			if (
				
				 (Position.MarketPosition == MarketPosition.Flat 
				 && (BarsSinceExitExecution() >= 1 || BarsSinceExitExecution() == -1) 
				 )
				 && (Close[0] > Open[0])
				 && (Close[0] > Close[1])
				 && (Close[1] > Open[1])
				 && (Close[2] <= Open[2])
				 // && (Close[0] > EMA2[0])
				
				 && (glv.GetAllRatioValues()[0] >= 4)
				 //GrimmerLegVol04.GetAllRatioValues()[0] >= 4
				 //(glv.GetAllRatioValues(0) >= 4) 
				
				 
				 && 
				 (
				
				 // Long 12 Set up 
				 (
				    (Low[1] < Low[2] )
				 && (Low[1] < Low[3] )
				 && (Low[1] < Low[4] )
				 && (Low[1] < Low[5] )
				 && (Low[1] < Low[6] )
				 )
				
				||
				
				 // Long 23 Set Up
				 ( 
				    (Low[2] < Low[3] ) 
				 && (Low[2] < Low[4] )
				 && (Low[2] < Low[5] )
				 && (Low[2] < Low[6] )
				 && (Low[2] < Low[7] ) 
				 )
								
				 )
								
				
				 && 
				
				( 
				
				(
				(Times[0][0].TimeOfDay >= new TimeSpan(09, 29, 59))
				&& (Times[0][0].TimeOfDay <= new TimeSpan(23, 00, 01))
				)
				//||
				//(
				//(Times[0][0].TimeOfDay >= new TimeSpan(10, 02, 00))
				//&& (Times[0][0].TimeOfDay <= new TimeSpan(10, 29, 59))
				//)
				
				
				)
				
				 && ( (Close[0] - MIN(Low,3)[0] )) <= (Tix)/(Tixperpoint)
				
				//(Tix*TickSize)/(Tixperpoint*TickSize)
				
				)
			{
				BackBrush = Brushes.Green;
				Draw.FibonacciRetracements(this, @"Fibs wdg02-ext Fibonacci retracements_1", false, 1, MIN(Low,3)[0], 0, Close[0]);
				Draw.Text(this, @"drawfibtest02 Text_1", Convert.ToString((Close[0] - (Low[1])) ), 0, (MIN(Low,3)[1] - 3) );
				//SetStopLoss("Long", CalculationMode.Ticks, Stop, false);
				//SetStopLoss("", CalculationMode.Price, Low[BarsSinceEntryExecution()+1]-1*TickSize, false);
				//SetProfitTarget("", CalculationMode.Price, Position.AveragePrice + ( Position.AveragePrice  - Low[BarsSinceEntryExecution()+2] ));
				//SetProfitTarget("", CalculationMode.Price, Position.AveragePrice + 30*TickSize); 
				
				
				// Calculate the target price
            	finalTargetl = Close[0] +  ( (Close[0] - MIN(Low,3)[0])*Target*0.25) ;

            	// Calculate the stop loss price
            	myStopl = MIN(Low,3)[0] - (Stop * TickSize);
				
				// Calculate the stop loss price
				breakevenTargetl = Close[0] + ((Close[0] - MIN(Low,3)[0]) * 0.45);
				
				// Calculate the target price
            	entryPrice = Close[0] ;
				
				EnterLong(Cars, "MyEntryLong");
				SetStopLoss(CalculationMode.Price, myStopl);
				SetProfitTarget("MyEntryLong", CalculationMode.Price, finalTargetl);
				
				myFreeTrade = true; ///Allow you to move your profit and stop freely if you want
				
				//ExitLongStopMarket(0, true, Position.Quantity, myStopl, "MyStopLong", "MyEntryLong");
				//ExitLongLimit(0, true, Position.Quantity, finalTargetl,  "MyTargetLong", "MyEntryLong");
				
				if (Position.MarketPosition == MarketPosition.Long && High[0] >= breakevenTargetl)
			{
				SetStopLoss(CalculationMode.Price, entryPrice-(0*TickSize) );
			}
				
				
				
				
            	// Place the order with the calculated target and stop loss
            	//EnterLong(1, "MyOrder", "MySignal", finalTargetl);
            	//SetStopLoss("MyOrder", CalculationMode.Price, myStopl);
	
				
				//EnterLong(Convert.ToInt32(DefaultQuantity), "MyEntryLong");
				
				
				
				
				
			}
			
			 // Set 2 - Long Exits
			if (
				Position.MarketPosition == MarketPosition.Long && myFreeTrade == true
				)
				// && (Position.GetUnrealizedProfitLoss(PerformanceUnit.Ticks, High[0]) >= Position.AveragePrice + ( Position.AveragePrice  - Low[BarsSinceEntryExecution()+3] )*0.5  ) )
			{
				//BackBrush = Brushes.Red;
				//Draw.Dot(this, @"QPPv02x02 Dot_1", false, 0, Position.AveragePrice, Brushes.LightSalmon);
				
				
				
				//myStop = Low[2] - (Stop * TickSize);
				//finalTarget = Close[1] + ((Close[1] - Low[2]) * Target); ///private double that sets your Final Target			
				
				//breakevenTarget = Close[1] + ((Close[1] - Low[2]) * 0.3);
				
				Print("myStopLong " + myStopl + " " + Time[0]);
				Print("finalTargetLong " + finalTargetl + " " + Time[0]);
				Print("breakevenTargetLong " + breakevenTargetl + " " + Time[0]);
				
				
				
				//ExitLongStopMarket(0, true, Position.Quantity, myStop, "MyStopLong", "MyEntryLong");
				//ExitLongLimit(0, true, Position.Quantity, finalTarget,  "MyTargetLong", "MyEntryLong");
				
				
				myFreeTrade = false;
				
				
			}
			
			
			
			
			
			
			if (Position.MarketPosition == MarketPosition.Long && High[0] >= breakevenTargetl)
			{
				SetStopLoss(CalculationMode.Price, entryPrice - (0*TickSize));
			}
			
			if (Position.MarketPosition == MarketPosition.Short && Low[0] <= breakevenTargets)
			{
				SetStopLoss(CalculationMode.Price, entryPrice + (0*TickSize));
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Target", Order=1, GroupName="Parameters")]
		public int Target
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Stop", Order=2, GroupName="Parameters")]
		public int Stop
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Cars", Order=3, GroupName="Parameters")]
		public int Cars
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Tix", Order=4, GroupName="Parameters")]
		public int Tix
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Tixperpoint", Order=5, GroupName="Parameters")]
		public int Tixperpoint
		{ get; set; }
		
		
		
		
		
		#endregion

	}
}
