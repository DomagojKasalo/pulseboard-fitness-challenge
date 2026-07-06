import { Component, computed, input } from '@angular/core';
import { DailyVolumePoint } from '../../core/models';

interface Cell {
  date: string;
  label: string;
  points: number;
  level: number;
  filler: boolean;
}

const WEEKS = 13;
const LEVEL_COLORS = [
  'rgba(255,255,255,0.05)',
  'rgba(52,211,153,0.25)',
  'rgba(52,211,153,0.45)',
  'rgba(52,211,153,0.70)',
  'rgba(52,211,153,1)',
];

@Component({
  selector: 'app-activity-heatmap',
  template: `
    <div class="flex gap-[3px] overflow-x-auto pb-1">
      @for (week of weeks(); track $index) {
        <div class="flex flex-col gap-[3px]">
          @for (cell of week; track $index) {
            <div
              class="h-3 w-3 rounded-[3px]"
              [style.background-color]="cell.filler ? 'transparent' : color(cell.level)"
              [title]="cell.filler ? '' : cell.label + ' — ' + cell.points + ' pts'"></div>
          }
        </div>
      }
    </div>
    <div class="mt-2 flex items-center justify-end gap-1.5 text-xs text-slate-500">
      <span>Less</span>
      @for (l of [0, 1, 2, 3, 4]; track l) {
        <span class="h-3 w-3 rounded-[3px]" [style.background-color]="color(l)"></span>
      }
      <span>More</span>
    </div>
  `,
})
export class ActivityHeatmapComponent {
  readonly data = input<DailyVolumePoint[]>([]);

  readonly weeks = computed(() => this.buildWeeks(this.data()));

  color(level: number): string {
    return LEVEL_COLORS[level] ?? LEVEL_COLORS[0];
  }

  private buildWeeks(data: DailyVolumePoint[]): Cell[][] {
    const byDate = new Map(data.map((d) => [d.date, d.points]));
    const max = data.reduce((m, d) => Math.max(m, d.points), 0);

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    // Start on the Monday of the week WEEKS ago.
    const start = new Date(today);
    start.setDate(start.getDate() - (WEEKS * 7 - 1));
    const dow = (start.getDay() + 6) % 7; // Monday = 0
    start.setDate(start.getDate() - dow);

    const weeks: Cell[][] = [];
    const cursor = new Date(start);
    for (let w = 0; w < WEEKS + 1; w++) {
      const week: Cell[] = [];
      for (let d = 0; d < 7; d++) {
        const iso = cursor.toISOString().slice(0, 10);
        const future = cursor > today;
        const points = byDate.get(iso) ?? 0;
        week.push({
          date: iso,
          label: cursor.toLocaleDateString(undefined, { day: 'numeric', month: 'short' }),
          points,
          level: this.levelFor(points, max),
          filler: future,
        });
        cursor.setDate(cursor.getDate() + 1);
      }
      weeks.push(week);
    }
    return weeks;
  }

  private levelFor(points: number, max: number): number {
    if (points <= 0 || max <= 0) return 0;
    const ratio = points / max;
    if (ratio > 0.66) return 4;
    if (ratio > 0.33) return 3;
    if (ratio > 0.1) return 2;
    return 1;
  }
}
