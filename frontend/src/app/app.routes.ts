import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'leaderboard', pathMatch: 'full' },
  {
    path: 'leaderboard',
    loadComponent: () => import('./features/leaderboard/leaderboard').then((m) => m.LeaderboardComponent),
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.DashboardComponent),
  },
  {
    path: 'dashboard/:userId',
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.DashboardComponent),
  },
  {
    path: 'log',
    loadComponent: () => import('./features/log-activity/log-activity').then((m) => m.LogActivityComponent),
  },
  { path: '**', redirectTo: 'leaderboard' },
];
