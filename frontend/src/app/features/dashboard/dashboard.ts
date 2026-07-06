import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexGrid,
  ApexLegend,
  ApexNonAxisChartSeries,
  ApexPlotOptions,
  ApexStroke,
  ApexTooltip,
  ApexXAxis,
  ApexYAxis,
  NgApexchartsModule,
} from 'ng-apexcharts';
import { ApiService } from '../../core/api.service';
import { Dashboard, UserSummary } from '../../core/models';
import { sportMeta } from '../../core/sports';

interface AreaChart {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  fill: ApexFill;
  stroke: ApexStroke;
  dataLabels: ApexDataLabels;
  grid: ApexGrid;
  tooltip: ApexTooltip;
  colors: string[];
}

interface DonutChart {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  colors: string[];
  legend: ApexLegend;
  dataLabels: ApexDataLabels;
  stroke: ApexStroke;
  plotOptions: ApexPlotOptions;
  tooltip: ApexTooltip;
}

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, NgApexchartsModule, DecimalPipe, DatePipe],
  templateUrl: './dashboard.html',
})
export class DashboardComponent {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  readonly userId = input<string>();

  readonly users = signal<UserSummary[]>([]);
  readonly dashboard = signal<Dashboard | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly favoriteSport = computed(() => {
    const top = this.dashboard()?.sportBreakdown[0];
    return top ? sportMeta(top.sport) : null;
  });

  readonly areaChart = computed<AreaChart>(() => {
    const volume = this.dashboard()?.volumeOverTime ?? [];
    return {
      series: [{ name: 'Points', data: volume.map((v) => [new Date(v.date).getTime(), v.points]) }],
      chart: {
        type: 'area',
        height: 300,
        toolbar: { show: false },
        background: 'transparent',
        fontFamily: 'Inter, sans-serif',
        animations: { speed: 500 },
      },
      colors: ['#34d399'],
      dataLabels: { enabled: false },
      stroke: { curve: 'smooth', width: 3 },
      fill: {
        type: 'gradient',
        gradient: { shadeIntensity: 1, opacityFrom: 0.4, opacityTo: 0.05, stops: [0, 100] },
      },
      xaxis: {
        type: 'datetime',
        labels: { style: { colors: '#64748b' } },
        axisBorder: { show: false },
        axisTicks: { show: false },
      },
      yaxis: { labels: { style: { colors: '#64748b' } } },
      grid: { borderColor: 'rgba(255,255,255,0.06)', strokeDashArray: 4 },
      tooltip: { theme: 'dark', x: { format: 'dd MMM yyyy' } },
    };
  });

  readonly donutChart = computed<DonutChart>(() => {
    const breakdown = this.dashboard()?.sportBreakdown ?? [];
    return {
      series: breakdown.map((s) => s.points),
      labels: breakdown.map((s) => sportMeta(s.sport).label),
      colors: breakdown.map((s) => sportMeta(s.sport).color),
      chart: {
        type: 'donut',
        height: 300,
        background: 'transparent',
        fontFamily: 'Inter, sans-serif',
      },
      legend: { position: 'bottom', labels: { colors: '#94a3b8' } },
      dataLabels: { enabled: true, formatter: (val: number) => `${Math.round(val)}%` },
      stroke: { colors: ['#0f1626'], width: 2 },
      plotOptions: {
        pie: {
          donut: {
            size: '68%',
            labels: {
              show: true,
              total: { show: true, label: 'Total pts', color: '#94a3b8' },
            },
          },
        },
      },
      tooltip: { theme: 'dark', y: { formatter: (val: number) => `${val} pts` } },
    };
  });

  constructor() {
    this.api.getUsers().subscribe({
      next: (users) => {
        this.users.set(users);
        if (!this.userId() && users.length) {
          this.router.navigate(['/dashboard', users[0].id], { replaceUrl: true });
        }
      },
      error: () => this.error.set('Could not reach the API.'),
    });

    effect(() => {
      const id = this.userId();
      if (id) {
        this.loadDashboard(id);
      }
    });
  }

  onUserChange(userId: string): void {
    this.router.navigate(['/dashboard', userId]);
  }

  private loadDashboard(userId: string): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getDashboard(userId).subscribe({
      next: (data) => {
        this.dashboard.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load this athlete’s dashboard.');
        this.loading.set(false);
      },
    });
  }

  meta = sportMeta;
}
