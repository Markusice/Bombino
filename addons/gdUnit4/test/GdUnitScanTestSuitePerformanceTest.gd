extends GdUnitTestSuite


func test_load_performance() -> void:
	var time = LocalTime.now()
	prints("Scan for test suites.")
	var test_suites := GdUnitTestSuiteScanner.new().scan("res://addons/gdUnit4/test/")
	assert_int(time.elapsed_since_ms())\
		.override_failure_message("Expecting the loading time overall is less than 10s")\
		.is_less(10*1000)
	prints("Scanning of %d test suites took" % test_suites.size(), time.elapsed_since())
	for ts in test_suites:
		ts.free()
