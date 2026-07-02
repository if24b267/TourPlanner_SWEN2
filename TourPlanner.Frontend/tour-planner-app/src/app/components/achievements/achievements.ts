import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Achievement } from '../../models/achievement.model';
import { AchievementService } from '../../services/achievement.service';

@Component({
  selector: 'app-achievements',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './achievements.html',
  styleUrls: ['./achievements.scss']
})
export class AchievementsComponent implements OnInit {
  achievements: Achievement[] = [];
  isOpen = false;

  constructor(private achievementService: AchievementService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.achievementService.getAll().subscribe({
      next: (data) => {
        this.achievements = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading achievements:', err)
    });
  }

  toggle(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen) this.load(); // beim Oeffnen neu laden, falls inzwischen was freigeschaltet wurde
  }

  get unlockedCount(): number {
    return this.achievements.filter(a => a.unlocked).length;
  }
}