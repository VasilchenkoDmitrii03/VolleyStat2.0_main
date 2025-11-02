"""
Модели игры, сета и счета
"""
from enum import IntEnum
from typing import List, Optional
from pydantic import BaseModel, Field
from .team import Team
from .sequence import Rally, RallySequence, VolleyActionSequence, VolleyActionSegmentSequence, RallyResult


class SetResult(IntEnum):
    """Результат сета"""
    UNDEFINED = -1
    NOT_FINISHED = 0
    WON = 1
    LOST = 2


class SegmentPhase(IntEnum):
    """Фаза розыгрыша"""
    RECEP_1 = 0  # Первый прием
    RECEP = 1    # Прием
    BREAK = 2    # Подача


class Score(BaseModel):
    """Счет"""
    left: int = Field(default=0, ge=0, description="Счет левой команды")
    right: int = Field(default=0, ge=0, description="Счет правой команды")
    set_length: int = Field(default=25, description="Длина сета")

    def is_finished(self) -> bool:
        """Проверить, закончен ли сет"""
        if abs(self.left - self.right) > 1 and (self.left >= self.set_length or self.right >= self.set_length):
            return True
        return False


class Set(BaseModel):
    """Сет"""
    rallies: RallySequence = Field(default_factory=RallySequence, description="Розыгрыши сета")
    current_phase: SegmentPhase = Field(default=SegmentPhase.RECEP, description="Текущая фаза")
    set_length: int = Field(default=25, description="Длина сета (очков для победы)")
    set_result: SetResult = Field(default=SetResult.UNDEFINED, description="Результат сета")
    current_score: Score = Field(default_factory=Score, description="Текущий счет")

    def __init__(self, **data):
        if 'current_score' not in data and 'set_length' in data:
            data['current_score'] = Score(set_length=data['set_length'])
        super().__init__(**data)

    def add(self, rally: Rally) -> None:
        """Добавить розыгрыш"""
        self.rallies.add(rally)
        self._set_start_phase()
        self.update_score(rally)

    def _set_start_phase(self) -> None:
        """Установить начальную фазу"""
        if self.current_phase != SegmentPhase.RECEP:
            return
        
        for i, rally in enumerate(self.rallies):
            if len(rally.segments) > 0 and len(rally.segments[0].actions) > 0:
                first_action = rally.segments[0].actions[0]
                if first_action.author_type.value in [0, 1]:  # Player or Opponent
                    from .action import VolleyActionType
                    if first_action.author_type.value == 1 or first_action.action_type == VolleyActionType.RECEPTION:
                        self.current_phase = SegmentPhase.RECEP_1
                    else:
                        self.current_phase = SegmentPhase.BREAK

    def update_score(self, rally: Rally) -> None:
        """Обновить счет"""
        if rally.rally_result == RallyResult.WON:
            self.current_score.left += 1
        elif rally.rally_result == RallyResult.LOST:
            self.current_score.right += 1

    def update_phase(self, phase: SegmentPhase) -> None:
        """Обновить фазу"""
        self.current_phase = phase

    def is_finished(self) -> SetResult:
        """Проверить, закончен ли сет"""
        if self.current_score.is_finished():
            if self.current_score.left > self.current_score.right:
                self.set_result = SetResult.WON
            else:
                self.set_result = SetResult.LOST
        return self.set_result

    def convert_to_segment_sequence(self) -> VolleyActionSegmentSequence:
        """Конвертировать в последовательность сегментов"""
        return self.rallies.convert_to_segment_sequence()

    def convert_to_sequence(self) -> VolleyActionSequence:
        """Конвертировать в последовательность действий"""
        return self.rallies.convert_to_action_sequence()


class GameResult(IntEnum):
    """Результат игры"""
    UNDEFINED = -1
    NOT_FINISHED = 0
    WON = 1
    LOST = 2


class ActionsMetricTypes(BaseModel):
    """Типы метрик для действий"""
    name: str = Field(..., description="Название набора метрик")
    data: dict = Field(default_factory=dict, description="Данные о метриках по типам действий")

    class Config:
        json_schema_extra = {
            "example": {
                "name": "Standard Metrics",
                "data": {}
            }
        }


class Game(BaseModel):
    """Игра (матч)"""
    sets: List[Set] = Field(default_factory=list, description="Сеты игры")
    set_lengths: List[int] = Field(..., description="Длины сетов")
    current_set_index: int = Field(default=-1, description="Индекс текущего сета")
    sets_to_win: int = Field(..., description="Количество сетов для победы")
    result: GameResult = Field(default=GameResult.NOT_FINISHED, description="Результат игры")
    youtube_url: str = Field(default="", description="URL видео на YouTube")
    actions_metric_types: ActionsMetricTypes = Field(..., description="Типы метрик действий")
    team: Team = Field(..., description="Команда")

    class Config:
        json_schema_extra = {
            "example": {
                "set_lengths": [25, 25, 25, 25, 15],
                "sets_to_win": 3,
                "youtube_url": "https://youtube.com/watch?v=example",
                "actions_metric_types": {
                    "name": "Standard",
                    "data": {}
                },
                "team": {
                    "name": "Зенит",
                    "description": "Волейбольная команда",
                    "players": []
                }
            }
        }

    def add_set(self, set_obj: Set) -> GameResult:
        """Добавить сет"""
        self.sets.append(set_obj)
        self.result = self._is_game_finished()
        self.current_set_index += 1
        return self.result

    def update_result(self) -> None:
        """Обновить результат игры"""
        self.result = self._is_game_finished()

    def _is_game_finished(self) -> GameResult:
        """Проверить, закончена ли игра"""
        lost = 0
        won = 0
        for set_obj in self.sets:
            if set_obj.set_result == SetResult.LOST:
                lost += 1
            elif set_obj.set_result == SetResult.WON:
                won += 1
        
        if lost >= self.sets_to_win:
            return GameResult.LOST
        elif won >= self.sets_to_win:
            return GameResult.WON
        return GameResult.NOT_FINISHED

    @property
    def current_set(self) -> Optional[Set]:
        """Получить текущий сет"""
        if 0 <= self.current_set_index < len(self.sets):
            return self.sets[self.current_set_index]
        return None

    @property
    def next_set_length(self) -> int:
        """Получить длину следующего сета"""
        try:
            return self.set_lengths[len(self.sets)]
        except IndexError:
            return 25

    def get_volley_action_sequence(self) -> VolleyActionSequence:
        """Получить последовательность всех действий"""
        result = VolleyActionSequence()
        for set_obj in self.sets:
            result.extend(set_obj.convert_to_sequence())
        return result

    def get_volley_action_segment_sequence(self) -> VolleyActionSegmentSequence:
        """Получить последовательность всех сегментов"""
        result = VolleyActionSegmentSequence()
        for set_obj in self.sets:
            result.extend(set_obj.convert_to_segment_sequence())
        return result

    def get_rally_sequence(self) -> RallySequence:
        """Получить последовательность всех розыгрышей"""
        result = RallySequence()
        for set_obj in self.sets:
            result.extend(set_obj.rallies)
        return result
