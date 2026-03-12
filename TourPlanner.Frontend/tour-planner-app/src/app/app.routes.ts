import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { TourListComponent } from './tour-list/tour-list.component';
import { TourDetailComponent } from './tour-detail/tour-detail.component';
import { TourLogComponent } from './tour-log/tour-log.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'tour-list', component: TourListComponent },
  { path: 'tour-detail', component: TourDetailComponent },
  { path: 'tour-log', component: TourLogComponent }
];