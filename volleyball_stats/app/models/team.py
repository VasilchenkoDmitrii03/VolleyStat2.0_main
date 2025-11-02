"""
Модель команды
"""
from typing import List, Optional, Tuple
from pydantic import BaseModel, Field
from .player import Player


class Team(BaseModel):
    """Модель волейбольной команды"""
    id: Optional[str] = Field(default=None, description="ID команды")
    name: str = Field(..., description="Название команды")
    description: str = Field(default="", description="Описание команды")
    main_color: Tuple[int, int, int] = Field(default=(0, 128, 0), description="Основной цвет команды (RGB)")
    libero_color: Tuple[int, int, int] = Field(default=(255, 165, 0), description="Цвет либеро (RGB)")
    players: List[Player] = Field(default_factory=list, description="Список игроков команды")

    class Config:
        json_schema_extra = {
            "example": {
                "name": "Зенит Казань",
                "description": "Профессиональная волейбольная команда",
                "main_color": [0, 0, 255],
                "libero_color": [255, 0, 0],
                "players": []
            }
        }

    def add_player(self, player: Player) -> None:
        """Добавить игрока в команду"""
        if player in self.players:
            raise ValueError(f"Команда {self.name} уже содержит игрока {player.to_table_string()}")
        self.players.append(player)

    def get_player_by_number(self, number: int) -> Optional[Player]:
        """Получить игрока по номеру"""
        for player in self.players:
            if player.number == number:
                return player
        return None

    def remove_player(self, number: int) -> bool:
        """Удалить игрока по номеру"""
        player = self.get_player_by_number(number)
        if player:
            self.players.remove(player)
            return True
        return False

    def get_players_by_amplua(self, amplua: int) -> List[Player]:
        """Получить игроков по амплуа"""
        return [p for p in self.players if p.amplua == amplua]
