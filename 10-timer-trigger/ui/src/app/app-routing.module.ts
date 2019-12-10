import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BestdealsComponent } from './components/bestdeals/bestdeals.component';


const routes: Routes = [
  { path: '', component: BestdealsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
