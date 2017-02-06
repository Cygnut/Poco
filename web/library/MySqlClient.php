<?php
class MySqlClientException extends Exception
{
	public function __construct($message, $code = 0, Exception $previous = null)
	{
		parent::__construct($message, $code, $previous);
	}
}

class MySqlClient
{
	private $conn;
	
	// Expects a map with entries:
	//   dbname
	//   host
	//   username
	//   password
	public function __construct($config)
	{
		// Create connection
		$this->conn = new mysqli(
			$config["host"], 
			$config["username"], 
			$config["password"], 
			$config["dbname"]
			);
		
		$this->checkConnection();
	}
	
	private function checkConnection()
	{
		// Check connection
		if ($this->conn->connect_error)
			throw new MySqlClientException("Connection failed: " . $this->conn->connect_error);
	}
	
	public function escapeString($str)
	{
		$this->checkConnection();
		
		return $this->conn->real_escape_string($str);
	}
	
	private function execSP($spName, $paramTypes, $params, $hasResults)
	{
		$this->checkConnection();
		
		$q = implode(',', array_fill(0, count($params), '?'));
		$stmt = $this->conn->prepare("call $spName($q);");
		if ($stmt === false)
			throw new MySqlClientException("Failed to prepare statement for SP $spName with error " . $this->conn->errno);
		
		try
		{
			// With call_user_func_array, array params must be references, not values! So create an array of references!
			$args = [];
			$args[] = & $paramTypes;
			
			for ($i = 0; $i < count($params); ++$i)
				$args[] = & $params[$i];
			
			$args = array_merge([ $paramTypes ], $params);
			call_user_func_array([$stmt, "bind_param"], $args);
			
			if ($stmt->execute() === FALSE)
				throw new MySqlClientException("Failed to execute statement for SP $spName with error " . $stmt->error);
			
			$rows = [];
			if ($hasResults)
			{
				$result = $stmt->get_result();
				
				if ($result === false)
					throw new MySqlClientException("Failed to get result for prepared statement for SP $spName with error " . $stmt->error);
				
				while ($row = $result->fetch_array())
					$rows[] = $row;
			}
		}
		finally
		{
			$stmt->close();
		}
		
		return $rows;
	}
	
	public function executeSP($spName, $paramTypes = "", $params = [])
	{
		$this->execSP($spName, $paramTypes, $params, false);
	}
	
	public function executeSPWithResult($spName, $paramTypes = "", $params = [])
	{
		return $this->execSP($spName, $paramTypes, $params, true);
	}
	
	public function __destruct()
	{
		try
		{
			$this->conn->close();
		}
		catch (Exception $e) {}
	}
}
?>