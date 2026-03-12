import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  username = '';
  password = '';

  constructor(private router: Router) {}

  register() {
    if(this.username && this.password) {
      alert('Account created!'); 
      this.router.navigate(['/login']); 
    } else {
      alert('Please fill in all fields');
    }
  }
}