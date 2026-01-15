import React, {useEffect, useState} from 'react';
import {useRootContext} from '../../contexts/RootContext';
import {getKwekerStats} from '../../../controllers/server/kweker';
import {MonthlyRevenueDto, DailyRevenueDto} from '../../../declarations/dtos/output/KwekerStatsOutputDto';

type ViewMode = 'month' | 'day';

export function RevenueChart() {
	const {t} = useRootContext();
	const [monthlyData, setMonthlyData] = useState<MonthlyRevenueDto[]>([]);
	const [dailyData, setDailyData] = useState<DailyRevenueDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [viewMode, setViewMode] = useState<ViewMode>('month');

	useEffect(() => {
		const fetchData = async () => {
			try {
				const response = await getKwekerStats();
				if (response.data && response.data.monthlyRevenue) {
					setMonthlyData(response.data.monthlyRevenue);
					setDailyData(response.data.dailyRevenue || []);
				}
			} catch (error) {
				console.error('Failed to fetch revenue data:', error);
			} finally {
				setLoading(false);
			}
		};

		fetchData();
	}, []);

	if (loading) {
		return (
			<section className="kweker-dashboard-section revenue-chart-section">
				<h2 className="section-title">Inkomsten overzicht</h2>
				<div className="chart-loading">
					<p>Loading...</p>
				</div>
			</section>
		);
	}

	const displayData = viewMode === 'month' ? monthlyData : dailyData;
	const dataPoints = viewMode === 'month' 
		? monthlyData.map(d => ({ label: d.monthName, revenue: d.revenue }))
		: dailyData.map(d => ({ label: d.dateLabel, revenue: d.revenue }));

	if (displayData.length === 0) {
		return (
			<section className="kweker-dashboard-section revenue-chart-section">
				<h2 className="section-title">Inkomsten overzicht</h2>
				<div className="empty-state">
					<i className="bi bi-graph-up" style={{fontSize: '3rem', opacity: 0.3}}></i>
					<p>Nog geen inkomsten data beschikbaar</p>
				</div>
			</section>
		);
	}

	// Calculate chart dimensions and scales
	const maxRevenue = Math.max(...dataPoints.map(d => d.revenue), 100);
	const chartWidth = 900;
	const chartHeight = 300;
	const padding = { top: 20, right: 20, bottom: 60, left: 70 };
	const innerWidth = chartWidth - padding.left - padding.right;
	const innerHeight = chartHeight - padding.top - padding.bottom;

	// Create points for the line
	const points = dataPoints.map((data, index) => {
		const x = padding.left + (index / (dataPoints.length - 1)) * innerWidth;
		const y = padding.top + innerHeight - (data.revenue / maxRevenue) * innerHeight;
		return { x, y, data };
	});

	// Create path for area fill
	const areaPath = `
		M ${padding.left} ${padding.top + innerHeight}
		${points.map(p => `L ${p.x} ${p.y}`).join(' ')}
		L ${padding.left + innerWidth} ${padding.top + innerHeight}
		Z
	`;

	// Create path for line
	const linePath = points.map((p, i) => `${i === 0 ? 'M' : 'L'} ${p.x} ${p.y}`).join(' ');

	// Y-axis labels
	const yAxisSteps = 5;
	const yAxisLabels = Array.from({length: yAxisSteps + 1}, (_, i) => {
		const value = (maxRevenue / yAxisSteps) * i;
		return {
			value,
			y: padding.top + innerHeight - (i / yAxisSteps) * innerHeight
		};
	});

	return (
		<section className="kweker-dashboard-section revenue-chart-section">
			<div className="section-header" style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem'}}>
				<h2 className="section-title">Inkomsten overzicht</h2>
				<div className="view-toggle" style={{display: 'flex', gap: '0.5rem'}}>
					<button 
						className={`toggle-btn ${viewMode === 'month' ? 'active' : ''}`}
						onClick={() => setViewMode('month')}
						style={{
							padding: '0.5rem 1rem',
							border: '1px solid #e5e7eb',
							borderRadius: '0.375rem',
							background: viewMode === 'month' ? '#10b981' : 'white',
							color: viewMode === 'month' ? 'white' : '#374151',
							cursor: 'pointer',
							fontWeight: viewMode === 'month' ? '600' : '400'
						}}
					>
						Maand
					</button>
					<button 
						className={`toggle-btn ${viewMode === 'day' ? 'active' : ''}`}
						onClick={() => setViewMode('day')}
						style={{
							padding: '0.5rem 1rem',
							border: '1px solid #e5e7eb',
							borderRadius: '0.375rem',
							background: viewMode === 'day' ? '#10b981' : 'white',
							color: viewMode === 'day' ? 'white' : '#374151',
							cursor: 'pointer',
							fontWeight: viewMode === 'day' ? '600' : '400'
						}}
					>
						Dag
					</button>
				</div>
			</div>

			<div className="chart-container" style={{backgroundColor: 'white'}}>
				<svg viewBox={`0 0 ${chartWidth} ${chartHeight}`} className="revenue-chart" style={{backgroundColor: 'white'}}>
					{/* White background */}
					<rect x="0" y="0" width={chartWidth} height={chartHeight} fill="white" />
					
					{/* Grid lines */}
					{yAxisLabels.map((label, i) => (
						<line
							key={i}
							x1={padding.left}
							y1={label.y}
							x2={chartWidth - padding.right}
							y2={label.y}
							stroke="#e5e7eb"
							strokeWidth="1"
						/>
					))}

					{/* Y-axis labels */}
					{yAxisLabels.map((label, i) => (
						<text
							key={i}
							x={padding.left - 10}
							y={label.y + 4}
							textAnchor="end"
							className="chart-label"
						>
							€{label.value.toFixed(0)}
						</text>
					))}

					{/* Area fill */}
					<path
						d={areaPath}
						fill="url(#gradient)"
						opacity="0.3"
					/>

					{/* Line */}
					<path
						d={linePath}
						fill="none"
						stroke="#10b981"
						strokeWidth="3"
						strokeLinecap="round"
						strokeLinejoin="round"
					/>

					{/* Data points */}
					{points.map((point, i) => (
						<g key={i}>
							<circle
								cx={point.x}
								cy={point.y}
								r="5"
								fill="#10b981"
								className="chart-point"
							/>
							<title>
								{point.data.label}: €{point.data.revenue.toFixed(2)}
							</title>
						</g>
					))}

					{/* X-axis labels */}
					{points.map((point, i) => {
						// For daily view show every 3rd label, for monthly show every 2nd
						const skipCount = viewMode === 'day' ? 3 : 2;
						if (i % skipCount === 0 || i === points.length - 1) {
							return (
								<text
									key={i}
									x={point.x}
									y={chartHeight - padding.bottom + 20}
									textAnchor="middle"
									className="chart-label"
								>
									{point.data.label}
								</text>
							);
						}
						return null;
					})}

					{/* Gradient definition */}
					<defs>
						<linearGradient id="gradient" x1="0%" y1="0%" x2="0%" y2="100%">
							<stop offset="0%" stopColor="#10b981" stopOpacity="0.8" />
							<stop offset="100%" stopColor="#10b981" stopOpacity="0.1" />
						</linearGradient>
					</defs>
				</svg>
			</div>

			<div className="chart-summary">
				<div className="summary-item">
					<span className="summary-label">{viewMode === 'month' ? 'Hoogste maand:' : 'Hoogste dag:'}</span>
					<span className="summary-value">
						€{Math.max(...dataPoints.map(d => d.revenue)).toFixed(2)}
					</span>
				</div>
				<div className="summary-item">
					<span className="summary-label">{viewMode === 'month' ? 'Gemiddelde per maand:' : 'Gemiddelde per dag:'}</span>
					<span className="summary-value">
						€{(dataPoints.reduce((sum, d) => sum + d.revenue, 0) / dataPoints.length).toFixed(2)}
					</span>
				</div>
				<div className="summary-item">
					<span className="summary-label">{viewMode === 'month' ? 'Totaal (12 maanden):' : 'Totaal (30 dagen):'}</span>
					<span className="summary-value">
						€{dataPoints.reduce((sum, d) => sum + d.revenue, 0).toFixed(2)}
					</span>
				</div>
			</div>
		</section>
	);
}
