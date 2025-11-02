"""
Модуль правил для сегментов действий (VolleyActionSegmentRules)
Определяет, какие действия могут следовать за другими, и результаты действий
Аналог StatisticsCreatorModule/Logic/VolleyActionSegmetRules.cs
"""
from typing import List, Dict
from app.models.action import VolleyActionType
from app.models.sequence import ActionSegmentResult


class VolleyActionSegmentRules:
    """
    Правила для последовательности действий в волейболе
    Определяет:
    1. Какие действия могут следовать после данного действия с определенным качеством
    2. Результат сегмента (выигрыш/проигрыш/не закончен) на основе действия и качества
    """
    
    def __init__(self, default_settings: bool = True):
        # sequence_logic[action_type][quality-1] = list of possible next actions
        self._sequence_logic: Dict[VolleyActionType, List[List[VolleyActionType]]] = {}
        
        # win_loss_logic[action_type][quality-1] = ActionSegmentResult
        self._win_loss_logic: Dict[VolleyActionType, List[ActionSegmentResult]] = {}
        
        if default_settings:
            self.set_default_rules()
    
    def set_rule_for_action_and_quality(self, 
                                        act_type: VolleyActionType, 
                                        quality: int, 
                                        *types: VolleyActionType) -> None:
        """
        Устанавливает правило: после действия act_type с качеством quality
        могут следовать действия types
        """
        if act_type not in self._sequence_logic:
            self._sequence_logic[act_type] = [[] for _ in range(6)]
        
        self._sequence_logic[act_type][quality - 1] = list(types)
    
    def set_rule_for_action_and_qualities(self,
                                         act_type: VolleyActionType,
                                         qualities: List[int],
                                         *types: VolleyActionType) -> None:
        """Устанавливает правило для нескольких качеств сразу"""
        for qual in qualities:
            self.set_rule_for_action_and_quality(act_type, qual, *types)
    
    def set_win_lose_rule(self, act_type: VolleyActionType, results: List[ActionSegmentResult]) -> None:
        """Устанавливает правило выигрыша/проигрыша для действия"""
        self._win_loss_logic[act_type] = results
    
    def set_default_rules(self) -> None:
        """Устанавливает стандартные правила волейбола"""
        
        # SERVE (Подача)
        self.set_rule_for_action_and_quality(
            VolleyActionType.SERVE, 6,  # Эйс
            VolleyActionType.SERVE
        )
        self.set_rule_for_action_and_qualities(
            VolleyActionType.SERVE, [5, 4, 3, 2],  # Хорошая подача
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, 
            VolleyActionType.FREE_BALL, VolleyActionType.SET, VolleyActionType.ATTACK
        )
        self.set_rule_for_action_and_quality(
            VolleyActionType.SERVE, 1,  # Ошибка подачи
            VolleyActionType.RECEPTION
        )
        
        # RECEPTION (Прием)
        self.set_rule_for_action_and_qualities(
            VolleyActionType.RECEPTION, [6, 5, 4, 3],  # Хороший прием
            VolleyActionType.SET, VolleyActionType.ATTACK, 
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, VolleyActionType.FREE_BALL
        )
        self.set_rule_for_action_and_quality(
            VolleyActionType.RECEPTION, 2,  # Плохой прием
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, 
            VolleyActionType.FREE_BALL, VolleyActionType.SET, VolleyActionType.ATTACK
        )
        self.set_rule_for_action_and_quality(
            VolleyActionType.RECEPTION, 1,  # Ошибка приема
            VolleyActionType.RECEPTION
        )
        
        # SET (Передача)
        self.set_rule_for_action_and_qualities(
            VolleyActionType.SET, [6, 5, 4, 3, 2],
            VolleyActionType.ATTACK, VolleyActionType.TRANSFER,
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, VolleyActionType.FREE_BALL
        )
        self.set_rule_for_action_and_quality(
            VolleyActionType.SET, 1,  # Ошибка передачи
            VolleyActionType.RECEPTION
        )
        
        # ATTACK (Атака)
        self.set_rule_for_action_and_quality(
            VolleyActionType.ATTACK, 6,  # Убойная атака
            VolleyActionType.SERVE
        )
        self.set_rule_for_action_and_qualities(
            VolleyActionType.ATTACK, [5, 4, 3],  # Атака в игру
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, 
            VolleyActionType.FREE_BALL, VolleyActionType.SET, VolleyActionType.ATTACK
        )
        self.set_rule_for_action_and_qualities(
            VolleyActionType.ATTACK, [2, 1],  # Ошибка атаки
            VolleyActionType.RECEPTION
        )
        
        # BLOCK (Блок)
        self.set_rule_for_action_and_quality(
            VolleyActionType.BLOCK, 6,  # Результативный блок
            VolleyActionType.SERVE
        )
        self.set_rule_for_action_and_qualities(
            VolleyActionType.BLOCK, [5, 4, 3, 2],  # Блок в игру
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, 
            VolleyActionType.FREE_BALL, VolleyActionType.SET, VolleyActionType.ATTACK
        )
        self.set_rule_for_action_and_quality(
            VolleyActionType.BLOCK, 1,  # Ошибка блока
            VolleyActionType.RECEPTION
        )
        
        # DEFENCE (Защита)
        self.set_rule_for_action_and_qualities(
            VolleyActionType.DEFENCE, [6, 5, 4, 3, 2],
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, 
            VolleyActionType.SET, VolleyActionType.ATTACK, VolleyActionType.TRANSFER
        )
        self.set_rule_for_action_and_quality(
            VolleyActionType.DEFENCE, 1,  # Ошибка защиты
            VolleyActionType.RECEPTION
        )
        
        # FREE_BALL (Свободный мяч)
        self.set_rule_for_action_and_qualities(
            VolleyActionType.FREE_BALL, [6, 5, 4, 3, 2],
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, 
            VolleyActionType.SET, VolleyActionType.ATTACK, VolleyActionType.TRANSFER
        )
        self.set_rule_for_action_and_quality(
            VolleyActionType.FREE_BALL, 1,
            VolleyActionType.RECEPTION
        )
        
        # TRANSFER (Перевод)
        self.set_rule_for_action_and_qualities(
            VolleyActionType.TRANSFER, [6, 5, 4, 3, 2],
            VolleyActionType.BLOCK, VolleyActionType.DEFENCE, 
            VolleyActionType.FREE_BALL, VolleyActionType.SET, VolleyActionType.ATTACK
        )
        self.set_rule_for_action_and_quality(
            VolleyActionType.TRANSFER, 1,
            VolleyActionType.RECEPTION
        )
        
        # OPPONENT ACTIONS (Действия соперника)
        self.set_rule_for_action_and_qualities(
            VolleyActionType.OPPONENT_POINT, [1, 2, 3, 4, 5, 6],
            VolleyActionType.RECEPTION
        )
        self.set_rule_for_action_and_qualities(
            VolleyActionType.OPPONENT_ERROR, [1, 2, 3, 4, 5, 6],
            VolleyActionType.SERVE
        )
        
        # Устанавливаем правила выигрыша/проигрыша
        self._set_default_win_lose_logic()
    
    def _set_default_win_lose_logic(self) -> None:
        """Устанавливает стандартные правила определения результата сегмента"""
        NE = ActionSegmentResult.NOT_ENDED
        WON = ActionSegmentResult.WON
        LOST = ActionSegmentResult.LOST
        
        self.set_win_lose_rule(VolleyActionType.SERVE, 
                              [LOST, NE, NE, NE, NE, WON])
        
        self.set_win_lose_rule(VolleyActionType.RECEPTION, 
                              [LOST, NE, NE, NE, NE, NE])
        
        self.set_win_lose_rule(VolleyActionType.SET, 
                              [LOST, NE, NE, NE, NE, NE])
        
        self.set_win_lose_rule(VolleyActionType.ATTACK, 
                              [LOST, LOST, NE, NE, NE, WON])
        
        self.set_win_lose_rule(VolleyActionType.BLOCK, 
                              [LOST, NE, NE, NE, NE, WON])
        
        self.set_win_lose_rule(VolleyActionType.DEFENCE, 
                              [LOST, NE, NE, NE, NE, NE])
        
        self.set_win_lose_rule(VolleyActionType.FREE_BALL, 
                              [LOST, NE, NE, NE, NE, NE])
        
        self.set_win_lose_rule(VolleyActionType.TRANSFER, 
                              [LOST, NE, NE, NE, NE, NE])
        
        self.set_win_lose_rule(VolleyActionType.OPPONENT_ERROR, 
                              [WON, WON, WON, WON, WON, WON])
        
        self.set_win_lose_rule(VolleyActionType.OPPONENT_POINT, 
                              [LOST, LOST, LOST, LOST, LOST, LOST])
    
    # ===== ИНТЕРФЕЙС ПОЛУЧЕНИЯ ПРАВИЛ =====
    
    def get_possible_actions(self, action_type: VolleyActionType, quality: int) -> List[VolleyActionType]:
        """Возвращает возможные действия после данного действия с указанным качеством"""
        if action_type in self._sequence_logic:
            return self._sequence_logic[action_type][quality - 1]
        return []
    
    def get_possible_actions_for_opponent(self, action_type: VolleyActionType) -> List[VolleyActionType]:
        """Возвращает возможные действия соперника (качество = 1)"""
        return self.get_possible_actions(action_type, 1)
    
    def get_action_result(self, action_type: VolleyActionType, quality: int) -> ActionSegmentResult:
        """Возвращает результат сегмента на основе действия и качества"""
        if action_type in self._win_loss_logic:
            return self._win_loss_logic[action_type][quality - 1]
        return ActionSegmentResult.NOT_ENDED
    
    def get_action_result_for_opponent(self, action_type: VolleyActionType) -> ActionSegmentResult:
        """Возвращает результат для действия соперника"""
        if action_type == VolleyActionType.OPPONENT_ERROR:
            return ActionSegmentResult.WON
        return ActionSegmentResult.LOST
    
    def get_action_result_for_judge(self, action_type: VolleyActionType) -> ActionSegmentResult:
        """Возвращает результат для действия судьи"""
        if action_type == VolleyActionType.DISPUTABLE_BALL:
            return ActionSegmentResult.DISPUTABLE
        if action_type == VolleyActionType.JUDGE_MISTAKE_LOST:
            return ActionSegmentResult.LOST
        return ActionSegmentResult.WON
    
    def get_set_start_players_actions(self) -> List[VolleyActionType]:
        """Возвращает действия игроков в начале сета"""
        return [VolleyActionType.SERVE, VolleyActionType.RECEPTION]
    
    def get_reception_players_actions(self) -> List[VolleyActionType]:
        """Возвращает действия приема"""
        return [VolleyActionType.RECEPTION]
