import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { LeaderboardEntry, Trend } from '../../core/models';

@Component({
  selector: 'app-leaderboard',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './leaderboard.html',
})
export class LeaderboardComponent {
  private readonly api = inject(ApiService);

  readonly entries = signal<LeaderboardEntry[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly podium = computed(() => {
    const top = this.entries().slice(0, 3);
    // Visual podium order: 2nd, 1st, 3rd
    return [top[1], top[0], top[2]].filter(Boolean);
  });
  readonly rest = computed(() => this.entries().slice(3));

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getLeaderboard().subscribe({
      next: (entries) => {
        this.entries.set(entries);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load the leaderboard. Is the API running?');
        this.loading.set(false);
      },
    });
  }

  initials(entry: LeaderboardEntry): string {
    return `${entry.firstName[0] ?? ''}${entry.lastName[0] ?? ''}`.toUpperCase();
  }

  trendIcon(trend: Trend): string {
    return { up: '▲', down: '▼', same: '—', new: '★' }[trend];
  }

  trendClass(trend: Trend): string {
    return {
      up: 'text-emerald-400',
      down: 'text-rose-400',
      same: 'text-slate-500',
      new: 'text-brand',
    }[trend];
  }

  trendLabel(entry: LeaderboardEntry): string {
    if (entry.trend === 'new') return 'NEW';
    if (entry.trend === 'same') return '±0';
    return `${Math.abs(entry.rankDelta)}`;
  }
}
