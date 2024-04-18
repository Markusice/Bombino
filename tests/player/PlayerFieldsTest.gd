class_name PlayerFieldsTest
extends GdUnitTestSuite
@warning_ignore('unused_parameter')
@warning_ignore('return_value_discarded')

var runner: GdUnitSceneRunner
var world_environment: Node
var player: Node

func before() -> void:
	runner = scene_runner("res://ui/game_loading_screen/game_loading_scene.tscn")

	var main = runner.invoke("find_child", "Main")
	world_environment = main.get_node("WorldEnvironment")

	await assert_signal(world_environment).is_emitted("EverythingLoaded")

	player = world_environment.get_node("PlayerBlue")
	assert_that(player).is_not_null()

func test_playerdata_is_set() -> void:
	var player_data = player.PlayerData

	assert_vector(player.position).is_equal_approx(player_data.Position, Vector3(0.004, 0.004, 0.004))

	assert_str(player.name).is_equal("PlayerBlue")

	assert_int(player_data.BombRange).is_equal(2)
	assert_int(player_data.NumberOfPlacedBombs).is_equal(0)
