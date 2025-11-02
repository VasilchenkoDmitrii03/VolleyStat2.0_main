"""
Модель игрока
"""
from enum import IntEnum
from typing import Optional
from pydantic import BaseModel, Field


class Amplua(IntEnum):
    """Амплуа игрока"""
    LIBERO = 0
    SETTER = 1
    OUTSIDE_HITTER = 2
    MIDDLE_BLOCKER = 3
    OPPOSITE = 4
    UNDEFINED = -1


class Player(BaseModel):
    """Модель игрока волейбольной команды"""
    name: str = Field(..., description="Имя игрока")
    surname: str = Field(..., description="Фамилия игрока")
    height: int = Field(..., ge=0, description="Рост игрока в см")
    number: int = Field(..., ge=0, description="Номер игрока")
    amplua: Amplua = Field(default=Amplua.UNDEFINED, description="Амплуа игрока")

    class Config:
        use_enum_values = False
        json_schema_extra = {
            "example": {
                "name": "Иван",
                "surname": "Иванов",
                "height": 195,
                "number": 10,
                "amplua": 2
            }
        }

    def to_table_string(self) -> str:
        """Представление для таблицы"""
        return f"{self.number} {self.surname} {self.name}"

    def __str__(self) -> str:
        return f"#{self.number}"

    def __eq__(self, other) -> bool:
        if not isinstance(other, Player):
            return False
        return self.number == other.number

    def __hash__(self):
        return hash(self.number)
