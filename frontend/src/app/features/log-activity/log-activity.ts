import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { IngestActivityRequest, SportKey, UserSummary } from '../../core/models';

type MetricType = 'distance' | 'duration' | 'count';
type Feedback = { kind: 'ok' | 'err'; text: string };

const SPORT_OPTIONS: { value: string; label: string; metric: MetricType }[] = [
  { value: 'running', label: '🏃 Running', metric: 'distance' },
  { value: 'walking', label: '🚶 Walking', metric: 'distance' },
  { value: 'cycling', label: '🚴 Cycling', metric: 'distance' },
  { value: 'gym', label: '🏋️ Gym', metric: 'duration' },
  { value: 'swimming', label: '🏊 Swimming', metric: 'duration' },
  { value: 'dailysteps', label: '👟 Daily Steps', metric: 'count' },
];

@Component({
  selector: 'app-log-activity',
  imports: [FormsModule, RouterLink],
  templateUrl: './log-activity.html',
})
export class LogActivityComponent {
  private readonly api = inject(ApiService);

  readonly sportOptions = SPORT_OPTIONS;
  readonly users = signal<UserSummary[]>([]);

  regFirst = '';
  regLast = '';
  readonly regBusy = signal(false);
  readonly regMsg = signal<Feedback | null>(null);

  actUser = '';
  actSport = 'running';
  actDatetime = this.defaultLocalDatetime();
  distance: number | null = null;
  duration = '';
  steps: number | null = null;
  readonly actBusy = signal(false);
  readonly actMsg = signal<Feedback | null>(null);
  readonly actErrors = signal<string[]>([]);

  constructor() {
    this.loadUsers();
  }

  get metric(): MetricType {
    return SPORT_OPTIONS.find((s) => s.value === this.actSport)?.metric ?? 'distance';
  }

  get metricValid(): boolean {
    if (this.metric === 'distance') return this.distance != null && this.distance > 0;
    if (this.metric === 'duration') return this.durationValid();
    return this.steps != null && this.steps > 0;
  }

  get canSubmit(): boolean {
    return !this.actBusy() && !!this.actUser && !!this.actDatetime && this.metricValid;
  }

  private durationValid(): boolean {
    const match = /^(\d+):([0-5]\d)$/.exec(this.duration.trim());
    return match != null && Number(match[1]) * 60 + Number(match[2]) > 0;
  }

  register(): void {
    this.regMsg.set(null);
    this.regBusy.set(true);
    this.api.registerUser(this.regFirst.trim(), this.regLast.trim()).subscribe({
      next: (user) => {
        this.regBusy.set(false);
        this.regMsg.set({ kind: 'ok', text: `Registered ${user.firstName} ${user.lastName}.` });
        this.regFirst = '';
        this.regLast = '';
        this.loadUsers(user.id);
      },
      error: (err: HttpErrorResponse) => {
        this.regBusy.set(false);
        const text = err.status === 409 ? 'That name is already taken.' : this.firstError(err) ?? 'Registration failed.';
        this.regMsg.set({ kind: 'err', text });
      },
    });
  }

  logActivity(): void {
    this.actMsg.set(null);
    this.actErrors.set([]);
    this.actBusy.set(true);

    const request: IngestActivityRequest = {
      userId: this.actUser,
      datetime: new Date(this.actDatetime).toISOString(),
    };

    if (this.metric === 'distance') {
      request.sport = this.actSport as SportKey;
      request.distance = this.distance ?? undefined;
    } else if (this.metric === 'duration') {
      request.sport = this.actSport as SportKey;
      request.duration = this.duration || undefined;
    } else {
      request.steps = this.steps ?? undefined;
    }

    this.api.ingestActivity(request).subscribe({
      next: (res) => {
        this.actBusy.set(false);
        this.actMsg.set({ kind: 'ok', text: `Logged! +${res.points} points awarded.` });
        this.resetMetrics();
      },
      error: (err: HttpErrorResponse) => {
        this.actBusy.set(false);
        this.actMsg.set({ kind: 'err', text: 'Could not log this activity.' });
        this.actErrors.set(this.allErrors(err));
      },
    });
  }

  private resetMetrics(): void {
    this.distance = null;
    this.duration = '';
    this.steps = null;
  }

  private loadUsers(select?: string): void {
    this.api.getUsers().subscribe((users) => {
      this.users.set(users);
      if (select) this.actUser = select;
      else if (!this.actUser && users.length) this.actUser = users[0].id;
    });
  }

  private firstError(err: HttpErrorResponse): string | null {
    return this.allErrors(err)[0] ?? null;
  }

  private allErrors(err: HttpErrorResponse): string[] {
    const errors = err.error?.errors as Record<string, string[]> | undefined;
    if (errors) return Object.values(errors).flat();
    if (err.error?.title) return [err.error.title];
    return [];
  }

  private defaultLocalDatetime(): string {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  }
}
