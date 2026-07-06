import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Dashboard,
  IngestActivityRequest,
  IngestActivityResponse,
  LeaderboardEntry,
  RegisterUserResponse,
  UserSummary,
} from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api';

  getUsers(): Observable<UserSummary[]> {
    return this.http.get<UserSummary[]>(`${this.base}/users`);
  }

  registerUser(firstName: string, lastName: string): Observable<RegisterUserResponse> {
    return this.http.post<RegisterUserResponse>(`${this.base}/users`, { firstName, lastName });
  }

  getLeaderboard(): Observable<LeaderboardEntry[]> {
    return this.http.get<LeaderboardEntry[]>(`${this.base}/leaderboard`);
  }

  getDashboard(userId: string): Observable<Dashboard> {
    return this.http.get<Dashboard>(`${this.base}/users/${userId}/dashboard`);
  }

  ingestActivity(request: IngestActivityRequest): Observable<IngestActivityResponse> {
    return this.http.post<IngestActivityResponse>(`${this.base}/activities`, request);
  }
}
