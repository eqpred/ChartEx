using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace System.Windows.Forms.DataVisualization.Charting {
	/// <summary>Windowsフォーム コントロール用のメソッドとプロパティが含まれています。</summary>
	[System.Runtime.CompilerServices.CompilerGenerated]
	internal class NamespaceDoc { }

	/// <summary>
	/// RecalculateAxesScaleRequiredイベントの内容を提供します。
	/// </summary>
	public class RecalculateAxesScaleRequiredEventArgs : EventArgs {
		/// <summary>
		/// Seriesの名前を取得します。
		/// </summary>
		public string SeriesName;

		/// <summary>
		/// 紐付いているChartAreaの名前を取得します。
		/// </summary>
		public string ChartAreaName;

		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		/// <param name="SeriesName">軸範囲を再計算するデータ名を保持します。</param>
		/// <param name="ChartAreaName">軸範囲を再計算するChartArea名を保持します。</param>
		public RecalculateAxesScaleRequiredEventArgs(string SeriesName, string ChartAreaName) {
			this.SeriesName = SeriesName;
			this.ChartAreaName = ChartAreaName;
		}
	}

	/// <summary>
	/// AxisTypeChangedイベントの内容を提供します。
	/// </summary>
	public class AxisTypeChangedEventArgs : EventArgs {
		/// <summary>
		/// SeriesまたはAxisの名前を取得します。
		/// </summary>
		public dynamic SourceName;

		/// <summary>
		/// 軸の名前を取得します。値はX,Y,X2またはY2です。
		/// </summary>
		public AxisName AxisName;

		/// <summary>
		/// 対数軸かどうかを取得します。
		/// </summary>
		public bool IsLogarithmic;

		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		/// <param name="SourceName">SeriesまたはAxisの名前を指定します。</param>
		/// <param name="AxisName">軸名を保持します。</param>
		/// <param name="IsLogarithmic">対数軸かどうかを保持します。</param>
		public AxisTypeChangedEventArgs(dynamic SourceName, AxisName AxisName, bool IsLogarithmic) {
			this.SourceName = SourceName;
			this.AxisName = AxisName;
			this.IsLogarithmic = IsLogarithmic;
		}
	}

	/*===========================================================================================================================*/
	/// <summary>
	/// 0以下の値があっても処理可能な機能を持ったAxisクラスを定義します。
	/// Axisは自身が紐付くChartArea・Chart・Seriesを知らないので、Chartを保持するコンポーネントのShownイベントで通知先を登録してください。
	/// </summary>
	public class AxisEx : Axis {

		/// <summary>
		/// 軸の設定値を保持します。
		/// </summary>
		public class AxisRange {
			/// <summary>
			/// 最大値を取得または設定します。
			/// </summary>
			public double Maximum { get; set; }

			/// <summary>
			/// 最小値を取得または設定します。
			/// </summary>
			public double Minimum { get; set; }

			/// <summary>
			/// 軸交差位置を取得または設定します。
			/// </summary>
			public double Crossing { get; set; }

			/// <summary>
			/// 初期を取得または設定します。
			/// </summary>
			public string Format { get; set; }

			/// <summary>
			/// 新しいインスタンスを生成します。
			/// </summary>
			/// <param name="Maximum">最大値を指定します。</param>
			/// <param name="Minimum">最小値を指定します。</param>
			/// <param name="Crossing">軸交差位置を指定します。</param>
			/// <param name="Format">書式を指定します。</param>
			public AxisRange(double Maximum = double.NaN, double Minimum = double.NaN, double Crossing = double.NaN, string Format = "") {
				this.Maximum = Maximum;
				this.Minimum = Minimum;
				this.Crossing = Crossing;
				this.Format = Format;
			}

			/// <summary>
			/// AxisRangeを表す文字列を取得します。
			/// </summary>
			/// <returns></returns>
			public override string ToString() {
				return $"Maximum={Maximum}, Minimum={Minimum}, Cross={Crossing}, Format={Format}";
			}
		}

		/*------------------------------------------------------------------*/
		#region フィールド
		/// <summary>
		/// 軸タイプを保持します。
		/// </summary>
		private bool isLogarithmic;

		/// <summary>
		/// 軸が線形の時の設定を保持します。
		/// </summary>
		private AxisRange rangeLinear;

		/// <summary>
		/// 軸が対数の時の設定を保持します。
		/// </summary>
		private AxisRange rangeLogarithm;
		#endregion

		/*------------------------------------------------------------------*/
		#region イベント定義
		/// <summary>
		/// 軸タイプが変更されたときに発生します。
		/// </summary>
		public event EventHandler<AxisTypeChangedEventArgs> AxisTypeChanged;
		#endregion

		/*------------------------------------------------------------------*/
		#region	初期化
		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		public AxisEx() : base() {
			Initialize();
		}

		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		/// <param name="chartArea">紐づけるChartAreaを指定します。</param>
		/// <param name="axisTypeName">軸の名前を指定します。</param>
		public AxisEx(ChartArea chartArea, AxisName axisTypeName) : base(chartArea, axisTypeName) {
			Initialize();
		}

		/// <summary>
		/// 初期化します。
		/// </summary>
		private void Initialize() {
			rangeLinear = new AxisRange();
			rangeLogarithm = new AxisRange();

			//親の設定を引き継ぐ
			isLogarithmic = base.IsLogarithmic;
			if(IsLogarithmic) {
				rangeLogarithm.Maximum = base.Maximum;
				rangeLogarithm.Minimum = base.Minimum;
				rangeLogarithm.Crossing = base.Crossing;
				rangeLogarithm.Format = base.LabelStyle.Format;
			} else {
				rangeLinear.Maximum = base.Maximum;
				rangeLinear.Minimum = base.Minimum;
				rangeLinear.Crossing = base.Crossing;
				rangeLinear.Format = base.LabelStyle.Format;
			}
			base.IsLogarithmic = false;//親の設定を線形に固定する
		}
		#endregion

		/*------------------------------------------------------------------*/
		#region プロパティ
		/// <summary>
		/// 軸が対数であるかどうかを示すフラグを取得または設定します。軸タイプを変更するとTitleが変更されます。
		/// </summary>
		public new bool IsLogarithmic {
			get => isLogarithmic;
			set {
				if(value != isLogarithmic) {
					isLogarithmic = value;
					var Matched = Regex.Match(Title, @"^((?<type>[Ll]([Oo][Gg]|[Nn]))[\(\[\<\{]{1})?(?<title>[a-zA-Z]{1}[\w\s/]+)[\)\]\>\}]?$");//現在のTitleを調べて
					Title = Matched.Success ? (isLogarithmic ? $"Log({Matched.Groups["title"].Value})" : Matched.Groups["title"].Value) : Title;//対数ならLogを付け、線形ならLogを外す
					AxisTypeChanged?.Invoke(this, new AxisTypeChangedEventArgs(this.Name, this.AxisName, isLogarithmic));
				}
			}
		}

		/// <summary>
		/// 保存された軸範囲情報を取得します。
		/// </summary>
		public AxisRange Range => isLogarithmic ? rangeLogarithm : rangeLinear;
		#endregion
	}

	/*===========================================================================================================================*/
	/// <summary>
	/// 格納できるデータ数を制限する機能を持つSeriesクラスを定義します。
	/// SeriesはChartAreaの名前は知っているがChartとAxisを知らないので、Chartを保持するコンポーネントのShownイベントで通知先を登録してください。
	/// </summary>
	public class SeriesEx : Series {
		/*------------------------------------------------------------------*/
		#region フィールド
		/// <summary>
		/// データ操作の排他制御に使用するトークンを保持します。
		/// </summary>
		private object lockToken;

		/// <summary>
		/// 格納できるデータ数の最大値を保持します。
		/// </summary>
		private int historyCapacity;

		/// <summary>
		/// 生データを保持します。
		/// </summary>
		private List<DataPoint> rawPoints;

		/// <summary>
		/// X軸が対数であるかどうかを保持します。
		/// </summary>
		private bool isXLogarithm;

		/// <summary>
		/// Y軸が対数であるかどうかを保持します。
		/// </summary>
		private bool isYLogarithm;
		#endregion

		/*------------------------------------------------------------------*/
		#region イベント定義
		/// <summary>
		/// 軸範囲の再計算が必要なときに発生します。
		/// </summary>
		public event EventHandler<RecalculateAxesScaleRequiredEventArgs> RecalculateAxesScaleRequired;

		/// <summary>
		/// 軸タイプが変更されたときに発生します。
		/// </summary>
		public event EventHandler<AxisTypeChangedEventArgs> AxisTypeChanged;

		/// <summary>
		/// データが更新されたときに発生します。
		/// </summary>
		public event EventHandler<DataPoint> PointUpdated;
		#endregion

		/*------------------------------------------------------------------*/
		#region 初期化 
		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		public SeriesEx() : base() {
			Initialize();
		}

		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		/// <param name="name">データの名称を指定します。</param>
		public SeriesEx(string name) : base(name) {
			Initialize();
		}

		/// <summary>
		/// 初期化を行います。
		/// </summary>
		private void Initialize() {
			lockToken = new object();
			rawPoints = new List<DataPoint>();
			historyCapacity = 1000;
		}
		#endregion

		/*------------------------------------------------------------------*/
		#region プロパティ
		/// <summary>
		/// 格納できるデータの最大数を取得または設定します。0以下の場合は無限に追加します。
		/// </summary>
		[Description("格納できるデータの最大数を設定します。0以下の場合は無限に追加します。")]
		public int HistoryCapacity {
			get => historyCapacity;
			set {
				lock(lockToken) {
					if(rawPoints.Count <= value) {
						historyCapacity = value;
					} else if(0 < value & value < rawPoints.Count) {
						Points.Clear();//表示データを初期化する
						rawPoints = rawPoints.Skip(rawPoints.Count - value).ToList();//差分数だけスキップして表示データに格納する
						rawPoints.ForEach(v => {
							Points.AddXY(
								isXLogarithm ? (0 < v.XValue ? Math.Log10(v.XValue) : double.NaN) : v.XValue,
								isYLogarithm ? (0 < v.YValues[0] ? Math.Log10(v.YValues[0]) : double.NaN) : v.YValues[0]);
						});
					} else if(value <= 0) {
						historyCapacity = 0;
					}
				}
			}
		}

		/// <summary>
		/// 格納されているデータ数を取得します。
		/// </summary>
		[Browsable(false)]
		public int HistoryCount => rawPoints.Count;

		/// <summary>
		/// X軸が対数軸かどうかを取得または設定します。
		/// </summary>
		[Description("X軸が対数軸かどうかを設定します。")]
		public bool IsXAxisLogarithm {
			get => isXLogarithm;
			set {
				if(value != isXLogarithm) {
					isXLogarithm = value;
					var an = (AxisName)Enum.GetValues(typeof(AxisName)).GetValue(this.XAxisType.Equals(AxisType.Primary) ? 0 : 1);
					AxisTypeChanged?.Invoke(this, new AxisTypeChangedEventArgs(this.Name, an, isXLogarithm));
					ReplacePoints();
				}
			}
		}

		/// <summary>
		/// Y軸が対数軸かどうかを取得または設定します。
		/// </summary>
		[Description("Y軸が対数軸かどうかを設定します。")]
		public bool IsYAxisLogarithm {
			get => isYLogarithm;
			set {
				if(value != isYLogarithm) {
					isYLogarithm = value;
					var an = (AxisName)Enum.GetValues(typeof(AxisName)).GetValue(this.YAxisType.Equals(AxisType.Primary) ? 1 : 3);
					AxisTypeChanged?.Invoke(this, new AxisTypeChangedEventArgs(this.Name, an, isYLogarithm));
					ReplacePoints();
				}
			}
		}

		/// <summary>
		/// 生データを取得します。
		/// </summary>
		[Description("生データを取得します。")]
		public List<DataPoint> RawPoints => new List<DataPoint>(rawPoints);
		#endregion

		/*------------------------------------------------------------------*/
		#region メソッド
		/// <summary>
		/// データを末尾に追加します。
		/// </summary>
		/// <param name="xValue">X軸上の値を指定します。</param>
		/// <param name="yValue">Y軸上の値を指定します。</param>
		public void AddXY(double xValue, double yValue) {
			lock(lockToken) {
				rawPoints.Add(new DataPoint(xValue, yValue));
				Points.AddXY(
					isXLogarithm ? (0 < xValue ? Math.Log10(xValue) : double.NaN) : xValue,
					isYLogarithm ? (object)(0 < yValue ? Math.Log10(yValue) : double.NaN) : (object)yValue
				);
				PointUpdated?.Invoke(this, RawPoints.Last());
			}
			if(0 < historyCapacity && rawPoints.Count == historyCapacity + 1) {//最大数が自然数で格納数がその値になったら先頭データを削除する
				lock(lockToken) {
					rawPoints.RemoveAt(0);
					Points.RemoveAt(0);
				}
				RecalculateAxesScaleRequired?.Invoke(this, new RecalculateAxesScaleRequiredEventArgs(this.Name, this.ChartArea));//軸範囲の再計算を要求する
			}
		}

		/// <summary>
		/// 指定位置のデータを更新します。
		/// </summary>
		/// <param name="xValue">X軸上の値を指定します。</param>
		/// <param name="yValue">Y軸上の新しい値を指定します。</param>
		public void UpdateXY(double xValue, double yValue) {
			var Found = rawPoints.Select((v, i) => new { i, v }).Where(a => a.v.XValue == xValue).Select(a => a.i);
			if(Found.Count() == 1) {
				int RefIndex = Found.First();
				lock(lockToken) {
					rawPoints[RefIndex].YValues[0] = yValue;
					Points[RefIndex].YValues[0] = isYLogarithm ? (0 < yValue ? Math.Log10(yValue) : double.NaN) : yValue;
				}
				RecalculateAxesScaleRequired?.Invoke(this, new RecalculateAxesScaleRequiredEventArgs(this.Name, this.ChartArea));//軸範囲の再計算を要求する
			}
		}

		/// <summary>
		/// 指定インデックスのデータを更新します。
		/// </summary>
		/// <param name="RefIndex">配列のインデックスを指定します。</param>
		/// <param name="yValue">Y軸上の新しい値を指定します。</param>
		public void UpdateXY(int RefIndex, double yValue) {
			if(0 <= RefIndex & RefIndex < rawPoints.Count) {
				lock(lockToken) {
					rawPoints[RefIndex].YValues[0] = yValue;
					Points[RefIndex].YValues[0] = isYLogarithm ? (0 < yValue ? Math.Log10(yValue) : double.NaN) : yValue;
				}
				RecalculateAxesScaleRequired?.Invoke(this, new RecalculateAxesScaleRequiredEventArgs(this.Name, this.ChartArea));//軸範囲の再計算を要求する
			}
		}

		/// <summary>
		/// 表示データの変更を行います。
		/// </summary>
		private void ReplacePoints() {
			lock(lockToken) {
				Points.Clear();
				rawPoints.ForEach(v => {
					Points.AddXY(
						isXLogarithm ? (0 < v.XValue ? Math.Log10(v.XValue) : double.NaN) : v.XValue,
						isYLogarithm ? (0 < v.YValues[0] ? Math.Log10(v.YValues[0]) : double.NaN) : v.YValues[0]);
				});
			}
			RecalculateAxesScaleRequired?.Invoke(this, new RecalculateAxesScaleRequiredEventArgs(this.Name, this.ChartArea));//軸範囲の再計算を要求する
		}

		/// <summary>
		/// データポイントを消去します。
		/// </summary>
		public void Clear() {
			rawPoints.Clear();
			Points.Clear();
		}
		#endregion
	}

	/*===========================================================================================================================*/
	/// <summary>
	/// 軸の更新イベントを処理できるChartAreaクラスを定義します。
	/// Seriesは自分が紐付くChartAreaの名前を知っているがオブジェクトは知らないので、Chartを保持するコンポーネントのShownイベントで通知先を登録してください。
	/// </summary>
	public class ChartAreaEx : ChartArea {
		/*------------------------------------------------------------------*/
		#region フィールド
		/// <summary>
		/// チャートエリアの位置と大きさを取得または設定します。軸とプロットを含みます。
		/// </summary>
		[Browsable(false)]
		public RectangleF ChartRect { get; set; }

		/// <summary>
		/// プロットエリアの位置と大きさを取得または設定します。データが表示される資格の領域のみを示します。
		/// </summary>
		[Browsable(false)]
		public RectangleF PlotRect { get; set; }

		/// <summary>
		/// 主軸のX軸の位置と大きさを取得または設定します。分割線・分割値・軸タイトルを含みます。
		/// </summary>
		[Browsable(false)]
		public RectangleF AxisXRect { get; set; }

		/// <summary>
		/// 主軸のY軸の位置と大きさを取得または設定します。分割線・分割値・軸タイトルを含みます。
		/// </summary>
		[Browsable(false)]
		public RectangleF AxisYRect { get; set; }

		/// <summary>
		/// 第2軸のX軸の位置と大きさを取得または設定します。分割線・分割値・軸タイトルを含みます。
		/// </summary>
		[Browsable(false)]
		public RectangleF AxisX2Rect { get; set; }

		/// <summary>
		/// 第2軸のY軸の位置と大きさを取得または設定します。分割線・分割値・軸タイトルを含みます。
		/// </summary>
		[Browsable(false)]
		public RectangleF AxisY2Rect { get; set; }

		#endregion

		/*------------------------------------------------------------------*/
		#region イベント定義
		#endregion

		/*------------------------------------------------------------------*/
		#region 初期化 
		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		public ChartAreaEx() : base() {
			Initialize();
		}

		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		/// <param name="name">ChartAreaの名前を指定します。</param>
		public ChartAreaEx(string name) : base(name) {
			Initialize();
		}

		/// <summary>
		/// 初期化します
		/// </summary>
		private void Initialize() { }
		#endregion

		/*------------------------------------------------------------------*/
		#region プロパティ
		/// <summary>
		/// 主軸のX軸を表すAxisExオブジェクトを取得または設定します。
		/// </summary>
		[Browsable(false)]
		public new AxisEx AxisX {
			get => (AxisEx)base.AxisX;
			set { base.AxisX = value; }
		}

		/// <summary>
		/// 第2軸のX軸を表すAxisExオブジェクトを取得または設定します。
		/// </summary>
		[Browsable(false)]
		public new AxisEx AxisX2 {
			get => (AxisEx)base.AxisX2;
			set { base.AxisX2 = value; }
		}

		/// <summary>
		/// 主軸のY軸を表すAxisExオブジェクトを取得または設定します。
		/// </summary>
		[Browsable(false)]
		public new AxisEx AxisY {
			get => (AxisEx)base.AxisY;
			set { base.AxisY = value; }
		}

		/// <summary>
		/// 第2軸のY軸を表すAxisExオブジェクトを取得または設定します。
		/// </summary>
		[Browsable(false)]
		public new AxisEx AxisY2 {
			get => (AxisEx)base.AxisY2;
			set { base.AxisY2 = value; }
		}
		#endregion

		/*------------------------------------------------------------------*/
		#region メソッド
		/// <summary>
		/// チャート要素のRectangleを更新します
		/// </summary>
		/// <param name="ClientRectangle">ChartAreaのクライアント領域を指定します。</param>
		public void UpdateElementRectangles(Rectangle ClientRectangle) {
			//チャートエリアの位置とサイズ
			ChartRect = new RectangleF() {
				X = ClientRectangle.Width * this.Position.X / 100,
				Y = ClientRectangle.Height * this.Position.Y / 100,
				Width = ClientRectangle.Width * this.Position.Width / 100,
				Height = ClientRectangle.Height * this.Position.Height / 100,
			};
			//プロットエリアの位置とサイズ
			PlotRect = new RectangleF() {
				X = (float)this.AxisX.ValueToPixelPosition(this.AxisX.Minimum),
				Y = (float)this.AxisY.ValueToPixelPosition(this.AxisY.Maximum),
				Width = (float)(this.AxisX.ValueToPixelPosition(this.AxisX.Maximum) - this.AxisX.ValueToPixelPosition(this.AxisX.Minimum)),
				Height = (float)(this.AxisY.ValueToPixelPosition(this.AxisY.Minimum) - this.AxisY.ValueToPixelPosition(this.AxisY.Maximum)),
			};
			//X軸エリアの位置とサイズ
			AxisXRect = new RectangleF() {
				X = PlotRect.X,
				Y = PlotRect.Y + PlotRect.Height,
				Width = PlotRect.Width,
				Height = (ChartRect.Y + ChartRect.Height) - (PlotRect.Y + PlotRect.Height),
			};
			//Y軸エリアの位置とサイズ
			AxisYRect = new RectangleF() {
				X = ChartRect.X,
				Y = PlotRect.Y,
				Width = PlotRect.X - ChartRect.X,
				Height = PlotRect.Height,
			};
			//X2軸エリアの位置とサイズ
			AxisX2Rect = new RectangleF() {
				X = PlotRect.X,
				Y = ChartRect.Y,
				Width = PlotRect.Width,
				Height = PlotRect.Y - ChartRect.Y,
			};
			//Y2軸エリアの位置とサイズ
			AxisY2Rect = new RectangleF() {
				X = PlotRect.X + PlotRect.Width,
				Y = PlotRect.Y,
				Width = (ChartRect.X + ChartRect.Width) - (PlotRect.X + PlotRect.Width),
				Height = PlotRect.Height,
			};
		}
		#endregion
	}

	/*===========================================================================================================================*/
	/// <summary>
	/// 軸の操作とデータ保持数の操作が可能なChartクラスを定義します。
	/// </summary>
	/// <example>
	/// ChartExコンポーネントの初期化例を示します。親ウィンドウの「Shown」で実施してください。
	/// <code>
	/// void InitializeChartEx() {
	/// 	//時系列データの場合
	///		((SeriesEx)chartPressure.Series[0]).HistoryCapacity = 100;//格納データ最大数を設定します
	///		chartPressure.InitializeStyle();//標準スタイルに設定します
	/// 	foreach(ChartAreaEx ca in chartPressure.ChartAreas) {//すべてのチャートエリアについて
	/// 		foreach(SeriesEx se in chartPressure.Series) {//すべてのデータ列について
	/// 			se.AxisTypeChanged += chartPressure.HandleAxisTypeChanged;//軸タイプ変更のイベントハンドラ
	/// 			se.RecalculateAxesScaleRequired += chartPressure.HandleRecalculateAxesScale;//軸範囲再計算のイベントハンドラ
	/// 		}
	/// 		foreach(AxisEx ae in ca.Axes)
	/// 			ae.AxisTypeChanged += chartPressure.HandleAxisTypeChanged;//軸タイプが変化した
	/// 	}
	/// 	//スペクトルデータの場合
	///		LastMass = 50;//最大質量数に合わせる
	///		chartSpectrum.InitializeStyle();
	///		foreach(ChartAreaEx ca in chartSpectrum.ChartAreas) {//チャートエリアについて
	///			foreach(SeriesEx se in chartSpectrum.Series) {
	///				se.AxisTypeChanged += chartSpectrum.HandleAxisTypeChanged;//軸タイプが変化した
	///				se.RecalculateAxesScaleRequired += chartSpectrum.HandleRecalculateAxesScale;//軸範囲の再計算
	///			}
	///			foreach(AxisEx ae in ca.Axes)
	///				ae.AxisTypeChanged += chartSpectrum.HandleAxisTypeChanged;//軸タイプが変化した
	///		}
	///		//軸タイプの初期設定は上記のイベントハンドラ登録が終わってから実施してください。順番が前後すると軸設定が反映されません。
	///		((AxisEx) chartSpectrum.ChartAreas[0].AxisY).IsLogarithmic = true;
	///		((AxisEx) chartSpectrum.ChartAreas[0].AxisY).Maximum = double.NaN;
	///		((AxisEx) chartSpectrum.ChartAreas[0].AxisY).Minimum = double.NaN;
	///		((AxisEx) chartSpectrum.ChartAreas[0].AxisY).Crossing = double.NaN;
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// ChartExコンポーネントにデータを追加する例を示します。
	/// <code>
	/// private void Clock_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
	/// 	var Elapsed = (DateTime.Now - StampOrigin).TotalSeconds;
	/// 	var Value = Randomizer.NextDouble();
	/// 		
	///		try {
	///			Invoke((MethodInvoker)delegate {
	///				if(chartPressure.Series["seriesPressure"].GetType().Equals(typeof(SeriesEx)))
	///					((SeriesEx) chartPressure.Series["seriesPressure"]).AddXY(Elapsed, Value);
	///				else
	///					chartPressure.Series["seriesPressure"].Points.AddXY(Elapsed, Value);
	/// 
	///				if(chartSpectrum.Series[0].Points.Count&lt;LastMass) {
	///					((SeriesEx) chartSpectrum.Series["seriesBlank"]).AddXY((double)++MassIndex, Math.Pow(10, Randomizer.NextDouble()));
	///					((SeriesEx) chartSpectrum.Series["seriesSample"]).AddXY((double) MassIndex, Math.Pow(10, Randomizer.NextDouble() * Randomizer.NextDouble() * 4));
	///				} else {
	///					((SeriesEx) chartSpectrum.Series["seriesBlank"]).UpdateXY((double)++MassIndex, Math.Pow(10, Randomizer.NextDouble()));
	///					((SeriesEx) chartSpectrum.Series["seriesSample"]).UpdateXY((double) MassIndex, Math.Pow(10, Randomizer.NextDouble() * Randomizer.NextDouble() * 4));
	///				}
	/// 			if(MassIndex == LastMass)
	/// 					MassIndex = 0;
	/// 		});
	/// 	} catch(Exception) { }
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	/// ChartExを使うときは以下の手順に従って下さい。
	/// 　1)参照設定でSystem.Windows.Forms.DataVisualization.Charingを追加します。
	/// 　2)ツールボックスからChartExコンポーネントをウィンドウに貼り付け、主要なプロパティを編集します。
	/// 　3)デザイナファイルを開け、本来は編集するべきでない「フォーム デザイナーで生成されたコード」の領域内の宣言を変更します。
	/// 　　　「Chart」⇒「ChartEx」、「ChartArea」⇒「ChartAreaEx」、「Series」⇒「SeriesEx」
	/// 　4)「AxisX」などは、newで「AxisEx」設定します。
	/// </remarks> 
	public class ChartEx : Chart {
		/*------------------------------------------------------------------*/
		#region	定義
		/// <summary>
		/// マウスイベントが発生したグラフ要素を定義します。
		/// </summary>
		public enum DrivenElementType {
			/// <summary>
			/// X軸
			/// </summary>
			X = 0,

			/// <summary>
			/// Y軸
			/// </summary>
			Y = 1,

			/// <summary>
			/// X2軸
			/// </summary>
			X2 = 2,

			/// <summary>
			/// Y2軸
			/// </summary>
			Y2 = 3,

			/// <summary>
			/// プロットエリア
			/// </summary>
			Plot = 4,

			/// <summary>
			/// グラフ要素以外
			/// </summary>
			Other = 5,
		}

		/// <summary>
		/// マウスイベントの情報を保持します。
		/// </summary>
		public class DrivenInfo {
			/// <summary>
			/// イベントが発生したクライアント領域の座標
			/// </summary>
			public readonly PointF Location;

			/// <summary>
			/// イベントが発生したチャートエリア
			/// </summary>
			public readonly ChartArea ChartArea;

			/// <summary>
			/// イベントが発生したグラフ要素を保持します。X,Y,X2,Y2,Plot,Otherのいずれかの値を取り、XからY2まではAxisNameと同じ番号が得られます。
			/// </summary>
			public readonly DrivenElementType DrivenElement;

			/// <summary>
			/// イベント情報を初期化します。
			/// </summary>
			/// <param name="Location">マウスイベントが発生した位置を指定します。</param>
			/// <param name="ChartArea">マウスイベントが発生したChartAreaを指定します。</param>
			/// <param name="DrivenElement">マウスイベントが発生したグラフ要素を指定します。</param>
			public DrivenInfo(PointF Location, ChartArea ChartArea, DrivenElementType DrivenElement) {
				this.Location = Location;
				this.ChartArea = ChartArea;
				this.DrivenElement = DrivenElement;
			}
		}
		#endregion

		/*------------------------------------------------------------------*/
		#region コンテキストメニューオブジェクト
		/// <summary>
		/// 軸操作に関するコンテキストメニューを保持します。
		/// </summary>
		private ContextMenuStrip contextMenu;

		/// <summary>
		/// 軸範囲の最大値を表示します。
		/// </summary>
		private ToolStripTextBox textBoxRangeMaximum;

		/// <summary>
		/// 軸範囲の最小値を表示します。
		/// </summary>
		private ToolStripTextBox textBoxRangeMinimum;

		/// <summary>
		/// 軸の交差値を表示します。
		/// </summary>
		private ToolStripTextBox textBoxRangeCrossing;

		/// <summary>
		/// 軸種別切りかえメニューを保持します。
		/// </summary>
		private ToolStripMenuItem menuLogarithm;

		/// <summary>
		/// 格納データ数変更メニューを保持します。
		/// </summary>
		private ToolStripMenuItem menuHistoryCapacity;

		/// <summary>
		/// 軸ラベルの書式を表示します。
		/// </summary>
		private ToolStripTextBox textBoxFormat;
		#endregion

		/*------------------------------------------------------------------*/
		#region	フィールド
		/// <summary>
		/// マウス操作の情報を保持します。
		/// </summary>
		private DrivenInfo drivenInfo;

		/// <summary>
		/// マウスホイールによるプロット領域のズーム倍率を保持します。
		/// </summary>
		private double scaleFactor;

		/// <summary>
		/// マウスドラッグ中のX軸方向のシフト量を保持します。
		/// </summary>
		private double shiftXPerPixel;

		/// <summary>
		/// マウスドラッグ中のY軸方向のシフト量を保持します。
		/// </summary>
		private double shiftYPerPixel;

		/// <summary>
		/// マウスドラッグ中のX2軸方向のシフト量を保持します。
		/// </summary>
		private double shiftX2PerPixel;

		/// <summary>
		/// マウスドラッグ中のY2軸方向のシフト量を保持します。
		/// </summary>
		private double shiftY2PerPixel;

		/// <summary>
		/// マウスがX軸領域をドラッグ中かどうかを保持します。
		/// </summary>
		private bool isDraggingXAxis;

		/// <summary>
		/// マウスがY軸領域をドラッグ中かどうかを保持します。
		/// </summary>
		private bool isDraggingYAxis;

		/// <summary>
		/// マウスがX2軸領域をドラッグ中かどうかを保持します。
		/// </summary>
		private bool isDraggingX2Axis;

		/// <summary>
		/// マウスがY2軸領域をドラッグ中かどうかを保持します。
		/// </summary>
		private bool isDraggingY2Axis;

		/// <summary>
		/// マウス移動量を計算する基準位置を保持します。
		/// </summary>
		private Point dragOrigin;

		/// <summary>
		/// コンテキストメニューの最初のクリック動作がおかしいために入れています。最初のUpイベントでtrueになります。
		/// </summary>
		private bool isContextMenuEnabled;
		#endregion

		/*------------------------------------------------------------------*/
		#region 初期化 
		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		public ChartEx() : base() {
			Initialize();
		}

		/// <summary>
		/// 初期化します
		/// </summary>
		private void Initialize() {
			scaleFactor = 0.15;

			#region コンテキストメニュー
			isContextMenuEnabled = false;

			contextMenu = new ContextMenuStrip {
				Name = "contextMenu",
				Size = new Size(181, 76)
			};
			contextMenu.Opening += ContextMenu_Opening;

			menuLogarithm = new ToolStripMenuItem {
				Name = "menuLogarithm",
				Size = new Size(180, 22),
				Text = "Logarithm",
				ToolTipText = "スケール種別を設定します。"
			};
			menuLogarithm.Click += MenuLogarithm_Click;

			textBoxRangeMaximum = new ToolStripTextBox {
				Name = "textBoxRangeMaximum",
				Size = new Size(100, 23),
				Text = "Maximum...",
				ToolTipText = "軸範囲の最大値を設定します。"
			};
			textBoxRangeMaximum.KeyDown += TextBoxRangeMaximum_KeyDown;
			textBoxRangeMaximum.DoubleClick += TextBoxRangeMaximum_DoubleClick;
			textBoxRangeMaximum.Click += TextBoxRangeMaximum_Click;
			textBoxRangeMaximum.LostFocus += TextBoxRangeMaximum_LostFocus;
			textBoxRangeMaximum.MouseUp += TextBoxRangeMaximum_MouseUp;
			textBoxRangeMaximum.GotFocus += TextBoxRangeMaximum_GotFocus;
			textBoxRangeMaximum.MouseHover += TextBoxRangeMaximum_MouseHover;
			textBoxRangeMaximum.MouseLeave += TextBoxRangeMaximum_MouseLeave;

			textBoxRangeMinimum = new ToolStripTextBox {
				Name = "textBoxRangeMinimum",
				Size = new Size(100, 23),
				Text = "Minimum...",
				ToolTipText = "軸範囲の最小値を設定します。"
			};
			textBoxRangeMinimum.KeyDown += TextBoxRangeMinimum_KeyDown;
			textBoxRangeMinimum.DoubleClick += TextBoxRangeMinimum_DoubleClick;
			textBoxRangeMinimum.Click += TextBoxRangeMinimum_Click;
			textBoxRangeMinimum.LostFocus += TextBoxRangeMinimum_LostFocus;
			textBoxRangeMinimum.MouseUp += TextBoxRangeMinimum_MouseUp;
			textBoxRangeMinimum.GotFocus += TextBoxRangeMinimum_GotFocus;
			textBoxRangeMinimum.MouseHover += TextBoxRangeMinimum_MouseHover;
			textBoxRangeMinimum.MouseLeave += TextBoxRangeMinimum_MouseLeave;

			textBoxRangeCrossing = new ToolStripTextBox {
				Name = "textBoxRangeCrossing",
				Size = new Size(100, 23),
				Text = "Crossing...",
				ToolTipText = "軸交差位置を設定します。"
			};
			textBoxRangeCrossing.KeyDown += TextBoxRangeCrossing_KeyDown;
			textBoxRangeCrossing.DoubleClick += TextBoxRangeCrossing_DoubleClick;
			textBoxRangeCrossing.Click += TextBoxRangeCrossing_Click;
			textBoxRangeCrossing.LostFocus += TextBoxRangeCrossing_LostFocus;
			textBoxRangeCrossing.MouseUp += TextBoxRangeCrossing_MouseUp;
			textBoxRangeCrossing.GotFocus += TextBoxRangeCrossing_GotFocus;
			textBoxRangeCrossing.MouseHover += TextBoxRangeCrossing_MouseHover;
			textBoxRangeCrossing.MouseLeave += TextBoxRangeCrossing_MouseLeave;

			textBoxFormat = new ToolStripTextBox {
				Name = "textBoxFormat",
				Size = new Size(100, 23),
				Text = "Format...",
				ToolTipText = "軸値の書式を設定します。"
			};
			textBoxFormat.KeyDown += TextBoxFormat_KeyDown;
			textBoxFormat.Click += TextBoxFormat_Click;
			textBoxFormat.LostFocus += TextBoxFormat_LostFocus;
			textBoxFormat.MouseUp += TextBoxFormat_MouseUp;
			textBoxFormat.GotFocus += TextBoxFormat_GotFocus;
			textBoxFormat.MouseHover += TextBoxFormat_MouseHover;
			textBoxFormat.MouseLeave += TextBoxFormat_MouseLeave;

			menuHistoryCapacity = new ToolStripMenuItem {
				Name = "menuHistoryCapacity",
				Size = new Size(180, 22),
				Text = "Capacity",
				ToolTipText = "格納データの最大数を設定します。"
			};

			ContextMenuStrip = contextMenu;
			#endregion
		}
		#endregion

		/*------------------------------------------------------------------*/
		#region プロパティ
		/// <summary>
		/// ホイール操作の倍率を取得または設定します。
		/// </summary>
		[Description("ホイール操作時の倍率を指定します。")]
		public double ScaleFactor {
			get => scaleFactor;
			set {
				if(0 < value & value <= 1)
					scaleFactor = value;
			}
		}
		#endregion

		/*------------------------------------------------------------------*/
		#region イベント処理
		#region マウス
		/// <summary>
		/// マウスボタンが押された時の処理を行います。
		/// </summary>
		/// <param name="e">マウスイベント情報を指定します。</param>
		protected override void OnMouseDown(MouseEventArgs e) {
			if(e.Button.Equals(MouseButtons.Left) & e.Clicks == 1) {
				drivenInfo = GetDrivenInfo(e.Location);
				if(drivenInfo != null) {
					if(drivenInfo.DrivenElement.Equals(DrivenElementType.X) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
						isDraggingXAxis = true;
						shiftXPerPixel = (double)(drivenInfo.ChartArea.AxisX.Maximum - drivenInfo.ChartArea.AxisX.Minimum) / ((ChartAreaEx)drivenInfo.ChartArea).PlotRect.Width;//ピクセルあたりのXシフト量
					}
					if(drivenInfo.DrivenElement.Equals(DrivenElementType.Y) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
						isDraggingYAxis = true;
						shiftYPerPixel = (double)(drivenInfo.ChartArea.AxisY.Maximum - drivenInfo.ChartArea.AxisY.Minimum) / ((ChartAreaEx)drivenInfo.ChartArea).PlotRect.Height;//ピクセルあたりのYシフト量
					}
					if(drivenInfo.DrivenElement.Equals(DrivenElementType.X2) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
						isDraggingX2Axis = true;
						shiftX2PerPixel = (double)(drivenInfo.ChartArea.AxisX2.Maximum - drivenInfo.ChartArea.AxisX2.Minimum) / ((ChartAreaEx)drivenInfo.ChartArea).PlotRect.Width;//ピクセルあたりのXシフト量
					}
					if(drivenInfo.DrivenElement.Equals(DrivenElementType.Y2) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
						isDraggingY2Axis = true;
						shiftY2PerPixel = (double)(drivenInfo.ChartArea.AxisY2.Maximum - drivenInfo.ChartArea.AxisY2.Minimum) / ((ChartAreaEx)drivenInfo.ChartArea).PlotRect.Height;//ピクセルあたりのYシフト量
					}
					dragOrigin = e.Location;
				}
				base.Cursor = Cursors.Hand;
			}
		}

		/// <summary>
		/// ダブルクリックが検出されたときの処理を行います。
		/// </summary>
		/// <param name="e">マウスイベント情報を指定します。</param>
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			if(e.Button.Equals(MouseButtons.Left) & e.Clicks == 2) {
				drivenInfo = GetDrivenInfo(e.Location);
				if(drivenInfo != null) {
					if(drivenInfo.DrivenElement.Equals(DrivenElementType.X) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
						drivenInfo.ChartArea.AxisX.Maximum = ((AxisEx)drivenInfo.ChartArea.AxisX).Range.Maximum;
						drivenInfo.ChartArea.AxisX.Minimum = ((AxisEx)drivenInfo.ChartArea.AxisX).Range.Minimum;
						drivenInfo.ChartArea.AxisX.Crossing = ((AxisEx)drivenInfo.ChartArea.AxisX).Range.Crossing;
						drivenInfo.ChartArea.AxisX.LabelStyle.Format = ((AxisEx)drivenInfo.ChartArea.AxisX).Range.Format;
					}
					if(drivenInfo.DrivenElement.Equals(DrivenElementType.Y) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
						drivenInfo.ChartArea.AxisY.Maximum = ((AxisEx)drivenInfo.ChartArea.AxisY).Range.Maximum;
						drivenInfo.ChartArea.AxisY.Minimum = ((AxisEx)drivenInfo.ChartArea.AxisY).Range.Minimum;
						drivenInfo.ChartArea.AxisY.Crossing = ((AxisEx)drivenInfo.ChartArea.AxisY).Range.Crossing;
						drivenInfo.ChartArea.AxisY.LabelStyle.Format = ((AxisEx)drivenInfo.ChartArea.AxisY).Range.Format;
					}
					if(drivenInfo.DrivenElement.Equals(DrivenElementType.X2) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
						drivenInfo.ChartArea.AxisX2.Maximum = ((AxisEx)drivenInfo.ChartArea.AxisX2).Range.Maximum;
						drivenInfo.ChartArea.AxisX2.Minimum = ((AxisEx)drivenInfo.ChartArea.AxisX2).Range.Minimum;
						drivenInfo.ChartArea.AxisX2.Crossing = ((AxisEx)drivenInfo.ChartArea.AxisX2).Range.Crossing;
						drivenInfo.ChartArea.AxisX2.LabelStyle.Format = ((AxisEx)drivenInfo.ChartArea.AxisX2).Range.Format;
					}
					if(drivenInfo.DrivenElement.Equals(DrivenElementType.Y2) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
						drivenInfo.ChartArea.AxisY2.Maximum = ((AxisEx)drivenInfo.ChartArea.AxisY2).Range.Maximum;
						drivenInfo.ChartArea.AxisY2.Minimum = ((AxisEx)drivenInfo.ChartArea.AxisY2).Range.Minimum;
						drivenInfo.ChartArea.AxisY2.Crossing = ((AxisEx)drivenInfo.ChartArea.AxisY2).Range.Crossing;
						drivenInfo.ChartArea.AxisY2.LabelStyle.Format = ((AxisEx)drivenInfo.ChartArea.AxisY2).Range.Format;
					}
				}
			}
		}

		/// <summary>
		/// マウスが動かされたときの処理を行います
		/// </summary>
		/// <param name="e">マウスイベント情報を指定します。</param>
		protected override void OnMouseMove(MouseEventArgs e) {
			if(drivenInfo != null) {
				if(isDraggingXAxis & e.Location != dragOrigin) {
					double ShiftX;
					ShiftX = shiftXPerPixel * (e.Location.X - dragOrigin.X);
					drivenInfo.ChartArea.AxisX.Minimum -= ShiftX;
					drivenInfo.ChartArea.AxisX.Maximum -= ShiftX;
				}
				if(isDraggingYAxis & e.Location != dragOrigin) {
					double ShiftY;
					ShiftY = shiftYPerPixel * (e.Location.Y - dragOrigin.Y);
					drivenInfo.ChartArea.AxisY.Minimum += ShiftY;
					drivenInfo.ChartArea.AxisY.Maximum += ShiftY;
				}
				if(isDraggingX2Axis & e.Location != dragOrigin) {
					double ShiftX;
					ShiftX = shiftX2PerPixel * (e.Location.X - dragOrigin.X);
					drivenInfo.ChartArea.AxisX2.Minimum -= ShiftX;
					drivenInfo.ChartArea.AxisX2.Maximum -= ShiftX;
				}
				if(isDraggingY2Axis & e.Location != dragOrigin) {
					double ShiftY;
					ShiftY = shiftY2PerPixel * (e.Location.Y - dragOrigin.Y);
					drivenInfo.ChartArea.AxisY2.Minimum += ShiftY;
					drivenInfo.ChartArea.AxisY2.Maximum += ShiftY;
				}
				dragOrigin = e.Location;
			}
			//base.OnMouseMove(e);
		}

		/// <summary>
		/// マウスボタンが放されたときの処理を行います。
		/// </summary>
		/// <param name="e">マウスイベント情報を指定します。</param>
		protected override void OnMouseUp(MouseEventArgs e) {
			base.Cursor = Cursors.Default;
			isDraggingXAxis = false;
			isDraggingYAxis = false;
			isDraggingX2Axis = false;
			isDraggingY2Axis = false;

			base.OnMouseUp(e);
		}

		/// <summary>
		/// マウスホイールが動かされたときの処理を行います。
		/// </summary>
		/// <param name="e">マウスイベント情報を指定します。</param>
		protected override void OnMouseWheel(MouseEventArgs e) {
			drivenInfo = GetDrivenInfo(e.Location);
			if(drivenInfo != null) {
				double Scale, Ratio;
				if(drivenInfo.DrivenElement.Equals(DrivenElementType.X) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
					Ratio = Math.Abs(e.X - ((ChartAreaEx)drivenInfo.ChartArea).AxisXRect.X) / ((ChartAreaEx)drivenInfo.ChartArea).AxisXRect.Width;
					Scale = Math.Abs(drivenInfo.ChartArea.AxisX.Maximum - drivenInfo.ChartArea.AxisX.Minimum) * scaleFactor;
					if(e.Delta < 0) {//Widen
						drivenInfo.ChartArea.AxisX.Minimum -= Scale * (1 - Ratio);
						drivenInfo.ChartArea.AxisX.Maximum += Scale * Ratio;
					} else {//Tighten
						drivenInfo.ChartArea.AxisX.Minimum += Scale * (1 - Ratio);
						drivenInfo.ChartArea.AxisX.Maximum -= Scale * Ratio;
					}
				}
				if(drivenInfo.DrivenElement.Equals(DrivenElementType.Y) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
					Ratio = Math.Abs(e.Y - ((ChartAreaEx)drivenInfo.ChartArea).AxisYRect.Y) / ((ChartAreaEx)drivenInfo.ChartArea).AxisYRect.Height;
					Scale = Math.Abs(drivenInfo.ChartArea.AxisY.Maximum - drivenInfo.ChartArea.AxisY.Minimum) * scaleFactor;
					if(e.Delta < 0) {//Widen
						drivenInfo.ChartArea.AxisY.Minimum -= Scale * (1 - Ratio);
						drivenInfo.ChartArea.AxisY.Maximum += Scale * Ratio;
					} else {//Tighten
						drivenInfo.ChartArea.AxisY.Minimum += Scale * (1 - Ratio);
						drivenInfo.ChartArea.AxisY.Maximum -= Scale * Ratio;
					}
				}
				if(drivenInfo.DrivenElement.Equals(DrivenElementType.X2) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
					Ratio = Math.Abs(e.X - ((ChartAreaEx)drivenInfo.ChartArea).AxisXRect.X) / ((ChartAreaEx)drivenInfo.ChartArea).AxisXRect.Width;
					Scale = Math.Abs(drivenInfo.ChartArea.AxisX2.Maximum - drivenInfo.ChartArea.AxisX2.Minimum) * scaleFactor;
					if(e.Delta < 0) {//Widen
						drivenInfo.ChartArea.AxisX2.Minimum -= Scale * (1 - Ratio);
						drivenInfo.ChartArea.AxisX2.Maximum += Scale * Ratio;
					} else {//Tighten
						drivenInfo.ChartArea.AxisX2.Minimum += Scale * (1 - Ratio);
						drivenInfo.ChartArea.AxisX2.Maximum -= Scale * Ratio;
					}
				}
				if(drivenInfo.DrivenElement.Equals(DrivenElementType.Y2) | drivenInfo.DrivenElement.Equals(DrivenElementType.Plot)) {
					Ratio = Math.Abs(e.Y - ((ChartAreaEx)drivenInfo.ChartArea).AxisYRect.Y) / ((ChartAreaEx)drivenInfo.ChartArea).AxisYRect.Height;
					Scale = Math.Abs(drivenInfo.ChartArea.AxisY2.Maximum - drivenInfo.ChartArea.AxisY2.Minimum) * scaleFactor;
					if(e.Delta < 0) {//Widen
						drivenInfo.ChartArea.AxisY2.Minimum -= Scale * (1 - Ratio);
						drivenInfo.ChartArea.AxisY2.Maximum += Scale * Ratio;
					} else {//Tighten
						drivenInfo.ChartArea.AxisY2.Minimum += Scale * (1 - Ratio);
						drivenInfo.ChartArea.AxisY2.Maximum -= Scale * Ratio;
					}
				}
			}
			//base.OnMouseWheel(e);
		}
		#endregion

		#region コンテキストメニュー
		/// <summary>
		/// コンテキストメニューが表示される前に処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void ContextMenu_Opening(object sender, CancelEventArgs e) {
			var menuStrip = (ContextMenuStrip)sender;
			var parentChart = menuStrip.SourceControl;
			var location = parentChart.PointToClient(menuStrip.Bounds.Location);
			drivenInfo = GetDrivenInfo(location);

			if(drivenInfo != null && (int)drivenInfo.DrivenElement < 4) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];

				contextMenu.Items.Clear();
				contextMenu.Items.AddRange(new ToolStripItem[] { textBoxRangeMaximum, textBoxRangeMinimum, textBoxRangeCrossing, textBoxFormat, new ToolStripSeparator() });
				if(axis.AxisName.Equals(AxisName.X) | axis.AxisName.Equals(AxisName.X)) {
					menuHistoryCapacity.DropDownItems.Clear();
					foreach(SeriesEx series in Series.Where(se => se.XAxisType.Equals(AxisType.Primary))) {
						var textBox = new ToolStripTextBox {
							Name = series.Name,
							Text = series.Name,
							Size = new System.Drawing.Size(100, 23)
						};
						textBox.KeyDown += TextBoxCapacity_KeyDown;
						textBox.Click += TextBoxCapacity_Click;
						textBox.LostFocus += TextBoxCapacity_LostFocus;
						textBox.MouseUp += TextBoxCapacity_MouseUp;
						textBox.GotFocus += TextBoxCapacity_GotFocus;
						textBox.MouseHover += TextBoxCapacity_MouseHover;
						textBox.MouseLeave += TextBoxCapacity_MouseLeave;
						menuHistoryCapacity.DropDownItems.Add(textBox);
					}
					contextMenu.Items.AddRange(new ToolStripItem[] { menuHistoryCapacity, new ToolStripSeparator() });
				}
				menuLogarithm.Checked = axis.IsLogarithmic;
				contextMenu.Items.Add(menuLogarithm);
				e.Cancel = false;
			} else
				e.Cancel = true;
		}

		#region Maximum
		/// <summary>
		/// コンテキストメニューのMaximumでキーが押されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMaximum_KeyDown(object sender, KeyEventArgs e) {
			if(drivenInfo != null && e.KeyCode == Keys.Enter) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				if(textBoxRangeMaximum.Text.Equals("") | textBoxRangeMaximum.Text.ToLower().Equals("nan"))
					axis.Maximum = double.NaN;
				else if(double.TryParse(textBoxRangeMaximum.Text, out var parsed) & axis.Minimum < parsed) {
					if(!axis.Range.Maximum.Equals(double.NaN))
						axis.Range.Maximum = parsed;
					axis.Maximum = parsed;
				}
			}
		}

		/// <summary>
		/// コンテキストメニューのMaximumでマウスが離されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMaximum_MouseUp(object sender, MouseEventArgs e) {
			if(isContextMenuEnabled == false) {//初回だけ動作します
				isContextMenuEnabled = true;
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeMaximum.Text = axis.Range.Maximum.Equals(double.NaN) ? axis.Maximum.ToString(axis.LabelStyle.Format) : axis.Range.Maximum.ToString(axis.LabelStyle.Format);
				textBoxRangeMaximum.ForeColor = axis.Range.Maximum.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのMaximumがフォーカスを失ったときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMaximum_LostFocus(object sender, EventArgs e) {
			textBoxRangeMaximum.Text = "Maximum...";
			textBoxRangeMaximum.ForeColor = Color.Black;
		}

		/// <summary>
		/// コンテキストメニューのMaximumでマウスがクリックされたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMaximum_Click(object sender, EventArgs e) {
			if(drivenInfo != null && ((MouseEventArgs)e).Button.Equals(MouseButtons.Left) & ((MouseEventArgs)e).Clicks == 1) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeMaximum.Text = axis.Range.Maximum.Equals(double.NaN) ? axis.Maximum.ToString(axis.LabelStyle.Format) : axis.Range.Maximum.ToString(axis.LabelStyle.Format);
				textBoxRangeMaximum.ForeColor = axis.Range.Maximum.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのMaximumtがフォーカスを得たときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMaximum_GotFocus(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeMaximum.Text = axis.Range.Maximum.Equals(double.NaN) ? axis.Maximum.ToString(axis.LabelStyle.Format) : axis.Range.Maximum.ToString(axis.LabelStyle.Format);
				textBoxRangeMaximum.ForeColor = axis.Range.Maximum.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのMaximumがダブルクリックされたときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeMaximum_DoubleClick(object sender, EventArgs e) {
			if(drivenInfo != null && isContextMenuEnabled & ((MouseEventArgs)e).Button.Equals(MouseButtons.Left)) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				if(axis.Range.Maximum.Equals(double.NaN)) {
					if(double.TryParse(textBoxRangeMaximum.Text, out double parsed)) {
						axis.Range.Maximum = parsed;
						textBoxRangeMaximum.ForeColor = Color.Red;
					}
				} else {
					axis.Range.Maximum = double.NaN;
					drivenInfo.ChartArea.RecalculateAxesScale();
					textBoxRangeMaximum.Text = axis.Maximum.ToString(axis.LabelStyle.Format);
					textBoxRangeMaximum.ForeColor = Color.Black;
				}
			}
		}

		/// <summary>
		/// コンテキストメニューのMaximumからマウスポインタが離れたときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeMaximum_MouseLeave(object sender, EventArgs e) {
			textBoxRangeMaximum.Text = "Maximum...";
			textBoxRangeMaximum.ForeColor = Color.Black;
		}

		/// <summary>
		/// コンテキストメニューのMaximumにマウスポインタが重なったときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeMaximum_MouseHover(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeMaximum.Text = axis.Range.Maximum.Equals(double.NaN) ? axis.Maximum.ToString(axis.LabelStyle.Format) : axis.Range.Maximum.ToString(axis.LabelStyle.Format);
				textBoxRangeMaximum.ForeColor = axis.Range.Maximum.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}
		#endregion

		#region Minimum
		/// <summary>
		/// コンテキストメニューのMinimumが操作されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMinimum_KeyDown(object sender, KeyEventArgs e) {
			if(drivenInfo != null && e.KeyCode == Keys.Enter) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				if(textBoxRangeMinimum.Text.Equals("") | textBoxRangeMinimum.Text.ToLower().Equals("nan"))
					axis.Minimum = double.NaN;
				else if(double.TryParse(textBoxRangeMinimum.Text, out var parsed) & parsed < axis.Maximum) {
					if(!axis.Range.Minimum.Equals(double.NaN))
						axis.Range.Minimum = parsed;
					axis.Minimum = parsed;
				}
			}
		}

		/// <summary>
		/// コンテキストメニューのMinimumでマウスが押されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMinimum_Click(object sender, EventArgs e) {
			if(drivenInfo != null && ((MouseEventArgs)e).Button.Equals(MouseButtons.Left) & ((MouseEventArgs)e).Clicks == 1) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeMinimum.Text = axis.Range.Minimum.Equals(double.NaN) ? axis.Minimum.ToString(axis.LabelStyle.Format) : axis.Range.Minimum.ToString(axis.LabelStyle.Format);
				textBoxRangeMinimum.ForeColor = axis.Range.Minimum.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのMinimumでマウスが離されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMinimum_MouseUp(object sender, MouseEventArgs e) {
			if(isContextMenuEnabled == false) {//初回だけ動作します
				isContextMenuEnabled = true;
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeMinimum.Text = axis.Range.Minimum.Equals(double.NaN) ? axis.Minimum.ToString(axis.LabelStyle.Format) : axis.Range.Minimum.ToString(axis.LabelStyle.Format);
				textBoxRangeMinimum.ForeColor = axis.Range.Minimum.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのMinimumがフォーカスを失ったときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMinimum_LostFocus(object sender, EventArgs e) {
			textBoxRangeMinimum.Text = "Minimum...";
			textBoxRangeMinimum.ForeColor = Color.Black;
		}

		/// <summary>
		/// コンテキストメニューのMinimumがダブルクリックされたときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeMinimum_DoubleClick(object sender, EventArgs e) {
			if(drivenInfo != null && isContextMenuEnabled & ((MouseEventArgs)e).Button.Equals(MouseButtons.Left)) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				if(axis.Range.Minimum.Equals(double.NaN)) {
					if(double.TryParse(textBoxRangeMinimum.Text, out double parsed)) {
						axis.Range.Minimum = parsed;
						textBoxRangeMinimum.ForeColor = Color.Red;
					}
				} else {
					axis.Range.Minimum = double.NaN;
					drivenInfo.ChartArea.RecalculateAxesScale();
					textBoxRangeMinimum.Text = axis.Minimum.ToString(axis.LabelStyle.Format);
					textBoxRangeMinimum.ForeColor = Color.Black;
				}
			}
		}

		/// <summary>
		/// コンテキストメニューのMinimumがフォーカスを得たときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeMinimum_GotFocus(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeMinimum.Text = axis.Range.Minimum.Equals(double.NaN) ? axis.Minimum.ToString(axis.LabelStyle.Format) : axis.Range.Minimum.ToString(axis.LabelStyle.Format);
				textBoxRangeMinimum.ForeColor = axis.Range.Minimum.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのMinimumからマウスポインタが離れたときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeMinimum_MouseLeave(object sender, EventArgs e) {
			textBoxRangeMinimum.Text = "Minimum...";
			textBoxRangeMinimum.ForeColor = Color.Black;
		}

		/// <summary>
		/// コンテキストメニューのMinimumにマウスポインタが重なったときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeMinimum_MouseHover(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeMinimum.Text = axis.Range.Minimum.Equals(double.NaN) ? axis.Minimum.ToString(axis.LabelStyle.Format) : axis.Range.Minimum.ToString(axis.LabelStyle.Format);
				textBoxRangeMinimum.ForeColor = axis.Range.Minimum.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}
		#endregion

		#region Crossing
		/// <summary>
		/// コンテキストメニューのCrossingが操作されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeCrossing_KeyDown(object sender, KeyEventArgs e) {
			if(drivenInfo != null && e.KeyCode == Keys.Enter) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				if(textBoxRangeCrossing.Text.Equals("") | textBoxRangeCrossing.Text.ToLower().Equals("nan"))
					axis.Crossing = double.NaN;
				else if(double.TryParse(textBoxRangeCrossing.Text, out var parsed)) {
					if(!axis.Range.Crossing.Equals(double.NaN))
						axis.Range.Crossing = parsed;
					axis.Crossing = parsed;
				}
			}
		}

		/// <summary>
		/// コンテキストメニューのCrossingでマウスがクリックされたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeCrossing_Click(object sender, EventArgs e) {
			if(drivenInfo != null && ((MouseEventArgs)e).Button.Equals(MouseButtons.Left) & ((MouseEventArgs)e).Clicks == 1) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeCrossing.Text = axis.Range.Crossing.Equals(double.NaN) ? axis.Crossing.ToString(axis.LabelStyle.Format) : axis.Range.Crossing.ToString(axis.LabelStyle.Format);
				textBoxRangeCrossing.ForeColor = axis.Range.Crossing.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのCrossingでマウスが離されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeCrossing_MouseUp(object sender, MouseEventArgs e) {
			if(isContextMenuEnabled == false) {//初回だけ動作します
				isContextMenuEnabled = true;
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeCrossing.Text = axis.Range.Crossing.Equals(double.NaN) ? axis.Crossing.ToString(axis.LabelStyle.Format) : axis.Range.Crossing.ToString(axis.LabelStyle.Format);
				textBoxRangeCrossing.ForeColor = axis.Range.Crossing.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのCrossingがフォーカスを失ったときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeCrossing_LostFocus(object sender, EventArgs e) {
			textBoxRangeCrossing.Text = "Crossing...";
			textBoxRangeCrossing.ForeColor = Color.Black;
		}

		/// <summary>
		/// コンテキストメニューのCrossingがフォーカスを得たときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxRangeCrossing_GotFocus(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeCrossing.Text = axis.Range.Crossing.Equals(double.NaN) ? axis.Crossing.ToString(axis.LabelStyle.Format) : axis.Range.Crossing.ToString(axis.LabelStyle.Format);
				textBoxRangeCrossing.ForeColor = axis.Range.Crossing.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}

		/// <summary>
		/// コンテキストメニューのCrossingがダブルクリックされたときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeCrossing_DoubleClick(object sender, EventArgs e) {
			if(drivenInfo != null && isContextMenuEnabled & ((MouseEventArgs)e).Button.Equals(MouseButtons.Left)) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				if(axis.Range.Crossing.Equals(double.NaN)) {
					if(double.TryParse(textBoxRangeCrossing.Text, out double parsed)) {
						axis.Range.Crossing = parsed;
						textBoxRangeCrossing.ForeColor = Color.Red;
					}
				} else {
					axis.Range.Crossing = double.NaN;
					drivenInfo.ChartArea.RecalculateAxesScale();
					textBoxRangeCrossing.Text = axis.Crossing.ToString(axis.LabelStyle.Format);
					textBoxRangeCrossing.ForeColor = Color.Black;
				}
			}
		}

		/// <summary>
		/// コンテキストメニューのCrossingからマウスポインタが離れたときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeCrossing_MouseLeave(object sender, EventArgs e) {
			textBoxRangeCrossing.Text = "Crossing...";
			textBoxRangeCrossing.ForeColor = Color.Black;
		}

		/// <summary>
		/// コンテキストメニューのCrossingにマウスポインタが重なったときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxRangeCrossing_MouseHover(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxRangeCrossing.Text = axis.Range.Crossing.Equals(double.NaN) ? axis.Crossing.ToString(axis.LabelStyle.Format) : axis.Range.Crossing.ToString(axis.LabelStyle.Format);
				textBoxRangeCrossing.ForeColor = axis.Range.Crossing.Equals(double.NaN) ? Color.Black : Color.Red;
			}
		}
		#endregion

		#region Format
		/// <summary>
		/// コンテキストメニューのFormatが操作されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxFormat_KeyDown(object sender, KeyEventArgs e) {
			if(drivenInfo != null && e.KeyCode == Keys.Enter) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				axis.LabelStyle.Format = textBoxFormat.Text != "" ? textBoxFormat.Text : "";
			}
		}

		/// <summary>
		/// コンテキストメニューのFormatでマウスがクリックされたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxFormat_Click(object sender, EventArgs e) {
			if(drivenInfo != null && ((MouseEventArgs)e).Button.Equals(MouseButtons.Left) & ((MouseEventArgs)e).Clicks == 1) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxFormat.Text = axis.LabelStyle.Format;
				textBoxFormat.ForeColor = axis.LabelStyle.Format == "" ? Color.Gray : Color.Black;
			}
		}

		/// <summary>
		/// コンテキストメニューのFormatでマウスが離されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxFormat_MouseUp(object sender, MouseEventArgs e) {
			if(isContextMenuEnabled == false) {//初回だけ動作します
				isContextMenuEnabled = true;
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxFormat.Text = axis.LabelStyle.Format;
				textBoxFormat.ForeColor = axis.LabelStyle.Format == "" ? Color.Gray : Color.Black;
			}
			isContextMenuEnabled = true;
		}

		/// <summary>
		/// コンテキストメニューのFormatがフォーカスを失ったときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxFormat_LostFocus(object sender, EventArgs e) {
			textBoxFormat.Text = "Format...";
			textBoxFormat.ForeColor = Color.Black;
		}

		/// <summary>
		/// コンテキストメニューのFormatがフォーカスを得たときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxFormat_GotFocus(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxFormat.Text = axis.LabelStyle.Format;
				textBoxFormat.ForeColor = axis.LabelStyle.Format == "" ? Color.Gray : Color.Black;
			}
		}

		/// <summary>
		/// コンテキストメニューのFormatからマウスポインタが離れたときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxFormat_MouseLeave(object sender, EventArgs e) {
			textBoxFormat.Text = "Format...";
			textBoxFormat.ForeColor = Color.Black;
		}

		/// <summary>
		/// コンテキストメニューのFormatにマウスポインタが重なったときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxFormat_MouseHover(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				textBoxFormat.Text = axis.LabelStyle.Format;
				textBoxFormat.ForeColor = axis.LabelStyle.Format == "" ? Color.Gray : Color.Black;
			}
		}
		#endregion

		#region HistoryCapacity
		/// <summary>
		/// コンテキストメニューのCapacityでキーが押されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxCapacity_KeyDown(object sender, KeyEventArgs e) {
			if(drivenInfo != null && e.KeyCode == Keys.Enter) {
				if(int.TryParse(((ToolStripTextBox)sender).Text, out int parsed) & parsed != ((SeriesEx)Series[((ToolStripTextBox)sender).Name]).HistoryCapacity)
					((SeriesEx)Series[((ToolStripTextBox)sender).Name]).HistoryCapacity = parsed;
			}
		}

		/// <summary>
		/// コンテキストメニューのCapacityでマウスが離されたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxCapacity_MouseUp(object sender, EventArgs e) {
			if(isContextMenuEnabled == false) {//初回だけ動作します
				isContextMenuEnabled = true;
				((ToolStripTextBox)sender).Text = ((SeriesEx)Series[((ToolStripTextBox)sender).Name]).HistoryCapacity.ToString();
			}
		}

		/// <summary>
		/// コンテキストメニューのCapacityでマウスがクリックされたときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxCapacity_Click(object sender, EventArgs e) {
			if(drivenInfo != null && ((MouseEventArgs)e).Button.Equals(MouseButtons.Left) & ((MouseEventArgs)e).Clicks == 1) {
				((ToolStripTextBox)sender).Text = ((SeriesEx)Series[((ToolStripTextBox)sender).Name]).HistoryCapacity.ToString();
			}
		}

		/// <summary>
		/// コンテキストメニューのCapacityがフォーカスを失ったときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxCapacity_LostFocus(object sender, EventArgs e) {
			((ToolStripTextBox)sender).Text = ((ToolStripTextBox)sender).Name;
		}

		/// <summary>
		/// コンテキストメニューのCapacityがフォーカスを得たときの処理を行います。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void TextBoxCapacity_GotFocus(object sender, EventArgs e) {
			if(drivenInfo != null) {
				((ToolStripTextBox)sender).Text = ((SeriesEx)Series[((ToolStripTextBox)sender).Name]).HistoryCapacity.ToString();
			}
		}

		/// <summary>
		/// コンテキストメニューのCapacityからマウスポインタが離れたときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxCapacity_MouseLeave(object sender, EventArgs e) {
			((ToolStripTextBox)sender).Text = ((ToolStripTextBox)sender).Name;
		}

		/// <summary>
		/// コンテキストメニューのCapacityにマウスポインタが重なったときの処理を行います。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxCapacity_MouseHover(object sender, EventArgs e) {
			if(drivenInfo != null) {
				((ToolStripTextBox)sender).Text = ((SeriesEx)Series[((ToolStripTextBox)sender).Name]).HistoryCapacity.ToString();
			}
		}
		#endregion

		#region Logarithm
		/// <summary>
		/// コンテキストメニューのLogarithmが選択されたときの処理を行います。軸の最大と最小は自動に設定されます。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">イベント情報を保持しています。</param>
		private void MenuLogarithm_Click(object sender, EventArgs e) {
			if(drivenInfo != null) {
				var axis = (AxisEx)drivenInfo.ChartArea.Axes[(int)drivenInfo.DrivenElement];
				if(axis.LabelStyle.Format != "")
					axis.Range.Format = axis.LabelStyle.Format;
				axis.IsLogarithmic = !axis.IsLogarithmic;

				OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, (int)drivenInfo.Location.X, (int)drivenInfo.Location.Y, 0));
			}
		}
		#endregion
		#endregion

		#region 軸
		/// <summary>
		/// 軸範囲を再計算します。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">RecalculateAxesScaleRequiredイベントの情報を保持しています。</param>
		public void HandleRecalculateAxesScale(object sender, RecalculateAxesScaleRequiredEventArgs e) {
			if(ChartAreas.FindByName(e.ChartAreaName) != null)//指定名のChartAreaがあれば
				ChartAreas[e.ChartAreaName].RecalculateAxesScale();//再計算を実施する
		}

		/// <summary>
		/// 軸タイプ変更イベントを処理します。
		/// </summary>
		/// <param name="sender">イベントを発生させたオブジェクトを保持しています。</param>
		/// <param name="e">AxisTypeChangedイベントの情報を保持しています。</param>
		public void HandleAxisTypeChanged(object sender, AxisTypeChangedEventArgs e) {
			if(sender.GetType().Equals(typeof(AxisEx))) {//AxisExが発信元なら
				switch(e.AxisName) {
					case AxisName.X:
						foreach(SeriesEx se in Series.Where(s => s.XAxisType.Equals(AxisType.Primary)))
							se.IsXAxisLogarithm = e.IsLogarithmic;
						break;
					case AxisName.X2:
						foreach(SeriesEx se in Series.Where(s => s.XAxisType.Equals(AxisType.Secondary)))
							se.IsXAxisLogarithm = e.IsLogarithmic;
						break;
					case AxisName.Y:
						foreach(SeriesEx se in Series.Where(s => s.YAxisType.Equals(AxisType.Primary)))
							se.IsYAxisLogarithm = e.IsLogarithmic;
						break;
					case AxisName.Y2:
						foreach(SeriesEx se in Series.Where(s => s.YAxisType.Equals(AxisType.Secondary)))
							se.IsYAxisLogarithm = e.IsLogarithmic;
						break;
				}
			}
			if(sender.GetType().Equals(typeof(SeriesEx))) {//SeriesExが発信元なら
				if(ChartAreas.FindByName(((SeriesEx)sender).ChartArea) != null)
					((AxisEx)ChartAreas[((SeriesEx)sender).ChartArea].Axes[(int)e.AxisName]).IsLogarithmic = e.IsLogarithmic;
			}
		}
		#endregion
		#endregion

		/*------------------------------------------------------------------*/
		#region メソッド
		/// <summary>
		/// 標準のスタイルに設定します。ChartExを使用するフォームのShownで実施してください。
		/// </summary>
		public void SetStandardStyle() {
			this.BackColor = Color.FromKnownColor(KnownColor.Control);
			this.ChartAreas.ToList().ForEach(ca => {
				ca.BorderDashStyle = ChartDashStyle.Solid;
				ca.Axes.ToList().ForEach(ax => {
					ax.MajorGrid.Enabled = false;
					ax.IsStartedFromZero = false;
					ax.LabelStyle.Font = new Font("Yu Gothic UI", 9F);
					ax.LabelStyle.Format = "0.0";
					//ax.IsLabelAutoFit = true;
					ax.TitleFont = new Font("Yu Gothic UI", 9F);
					if(ax.AxisName.Equals(AxisName.Y) | ax.AxisName.Equals(AxisName.Y2)) {
						var series = Series.ToList().Where(s => s.ChartArea.Equals(ca.Name));
						if(series.Count() == 1)
							ax.TitleForeColor = series.First().Color.IsEmpty ? Color.Black : series.First().Color;
					}
				});
			});
		}

		/// <summary>
		/// ChartExのイベントハンドラを設定します。ChartExを使用するフォームのShownで実施してください。
		/// </summary>
		public void SetEventHandlers() {
			//foreach(var ca in ChartAreas) {
			//	if(ca.GetType().Equals(typeof(ChartAreaEx))) {
			//		foreach(var se in Series) {//すべてのデータ列について
			//			if(se.GetType().Equals(typeof(SeriesEx))) {
			//				((SeriesEx)se).AxisTypeChanged += this.HandleAxisTypeChanged;//軸タイプ変更のイベントハンドラ
			//				((SeriesEx)se).RecalculateAxesScaleRequired += this.HandleRecalculateAxesScale;//軸範囲再計算のイベントハンドラ
			//			}
			//		}
			//		foreach(var ae in ca.Axes)
			//			if(ae.GetType().Equals(typeof(AxisEx)))
			//				((AxisEx)ae).AxisTypeChanged += this.HandleAxisTypeChanged;//軸タイプが変化した
			//	}
			//}
			foreach(ChartAreaEx ca in ChartAreas) {
				if(ca.GetType().Equals(typeof(ChartAreaEx))) {
					foreach(SeriesEx se in Series) {//すべてのデータ列について
						if(se.GetType().Equals(typeof(SeriesEx))) {
							((SeriesEx)se).AxisTypeChanged += this.HandleAxisTypeChanged;//軸タイプ変更のイベントハンドラ
							((SeriesEx)se).RecalculateAxesScaleRequired += this.HandleRecalculateAxesScale;//軸範囲再計算のイベントハンドラ
						}
					}
					foreach(AxisEx ae in ca.Axes)
						if(ae.GetType().Equals(typeof(AxisEx)))
							((AxisEx)ae).AxisTypeChanged += this.HandleAxisTypeChanged;//軸タイプが変化した
				}
			}
		}

		/// <summary>
		/// 発生したマウスイベントの情報を取得します。
		/// </summary>
		/// <param name="Location">マウス位置を指定します。</param>
		/// <returns>DrivenInfoを返します。</returns>
		private DrivenInfo GetDrivenInfo(Point Location) {
			foreach(ChartAreaEx chartArea in ChartAreas) {
				chartArea.UpdateElementRectangles(ClientRectangle);//チャート内の要素情報を更新する
				if(chartArea.PlotRect.Contains(Location))
					return new DrivenInfo(Location, chartArea, DrivenElementType.Plot);
				else if(chartArea.AxisXRect.Contains(Location))
					return new DrivenInfo(Location, chartArea, DrivenElementType.X);
				else if(chartArea.AxisYRect.Contains(Location))
					return new DrivenInfo(Location, chartArea, DrivenElementType.Y);
				else if(chartArea.AxisX2Rect.Contains(Location))
					return new DrivenInfo(Location, chartArea, DrivenElementType.X2);
				else if(chartArea.AxisY2Rect.Contains(Location))
					return new DrivenInfo(Location, chartArea, DrivenElementType.Y2);
			}
			return null;
		}
		#endregion
	}

}
