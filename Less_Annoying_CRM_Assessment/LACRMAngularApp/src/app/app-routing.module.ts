import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApiLogTableComponent } from './pages/api-log-table/api-log-table.component';

const routes: Routes = [
  { path: '', redirectTo: '/logs', pathMatch: 'full' }, // default route
  { path: 'logs', component: ApiLogTableComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
