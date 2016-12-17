<?php
    try
    {
        require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
                
        $data = json_decode(file_get_contents('php://input'), true);
        
        $id = (int) $data["id"];
        $direction = (int) $data["direction"];
        
        // Invoke query:
        $conn = new MySqlClient($config["db"]);
        $rows = $conn->executeQuery(
            "CALL c_voteEntity(@_id := $id, @_direction := $direction, @_by := 1);"
            );
        
        // Return the new score.
        echo json_encode(
            array(
                "id" => $id,
                "score" => $rows[0]["score"]
                )
            );
    }
    catch (Exception $e)
    {
        error_log($e->getMessage());
        echo createErrorResponse();
    }
?>
