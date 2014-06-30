	<?php
	/*!
	* GsnApiClient
	* https://github.com/gsn/ClientApiDoc/tree/master/Examples/php
	* (c) 2014, Tom Noogen | http://www.groceryshopping.net 
	*/

	// A service client for the GsnApi work flow/OAuth 2 mixed
	class GsnApiClient
	{
		public $api_base_url     = "";
		public $token_url        = "";
		public $token_info_url   = "";

		public $client_id        = "" ;
		public $client_secret    = "" ;
		public $access_token     = "" ;
		public $refresh_token    = "" ;

		public $access_token_expires_in = "" ;
		public $access_token_expires_at = "" ;

		//--

		public $sign_token_name          = "access_token";
		public $decode_json              = true;
		public $curl_time_out            = 30;
		public $curl_connect_time_out    = 30;
		public $curl_ssl_verifypeer      = false;
		public $curl_useragent           = "Gsn Simple PHP Api Client v0.1";
		public $curl_authenticate_method = "POST";
	        public $curl_proxy               = null;

		//--

		public $http_code             = "";
		public $http_info             = "";

		//--

		public function __construct( $api_base_url = "", $client_id = false, $client_secret = false)
		{
			$this->api_base_url  = rtrim($api_base_url, "/");
			$this->client_id     = $client_id;
			$this->client_secret = $client_secret;
			$this->token_url     = $api_base_url."/auth/Token2";    
		}

		public function authenticate()
		{
			$params = array(
				"client_id"     => $this->client_id,
				"client_secret" => $this->client_secret,
				"grant_type"    => "client_credentials"
			);
			$response = $this->request( $this->token_url, $params, $this->curl_authenticate_method );
			$response = $this->parseRequestResult( $response );

			if( ! $response || ! isset( $response->access_token ) ){
				throw new Exception( "The Authorization Service has returned in error.");
			}

			if( isset( $response->access_token  ) )  $this->access_token           = $response->access_token;
			if( isset( $response->refresh_token ) ) $this->refresh_token           = $response->refresh_token; 
			if( isset( $response->expires_in    ) ) $this->access_token_expires_in = $response->expires_in; 
			
			// calculate when the access token expire
			if( isset($response->expires_in)) {
				$this->access_token_expires_at = time() + $response->expires_in;
			}

			return $response;  
		}

		public function authenticated()
		{
			if ( $this->access_token ){
				if ( $this->token_info_url && $this->refresh_token ){
					// check if this access token has expired, 
					$tokeninfo = $this->tokenInfo( $this->access_token ); 

					// if yes, access_token has expired, then ask for a new one
					if( $tokeninfo && isset( $tokeninfo->error ) ){
						$response = $this->refreshToken( $this->refresh_token ); 

						// if wrong response
						if( ! isset( $response->access_token ) || ! $response->access_token ){
							throw new Exception( "The Authorization Service has return an invalid response while requesting a new access token. given up!" ); 
						}

						// set new access_token
						$this->access_token = $response->access_token; 
					}
				}
				// expired?
				else if( $this->access_token_expires_at <= time() ){
					$response = $this->refreshToken( $this->refresh_token ); 

                    // if wrong response
                    if( ! isset( $response->access_token ) || ! $response->access_token ){
	                     throw new Exception( "The Authorization Service has return an invalid response while requesting a new access token. given up!" );
	                 }
	                 // set new access_token
	                 $this->access_token = $response->access_token;
	            }      

				return true;
			}

			return false;
		}

		/** 
		* Format and sign an oauth for provider api 
		*/
		public function api( $url, $method = "GET", $parameters = array(), $headers = array() ) 
		{
			if (!$this->authenticated()) return null;
			
			if ( strrpos($url, 'http://') !== 0 && strrpos($url, 'https://') !== 0 ) {
				$url = $this->api_base_url . $url;
			}

			$response = null;

			switch( $method ){
				case 'GET'  : $response = $this->request( $url, $parameters, "GET", $headers  ); break; 
				case 'POST' : $response = $this->request( $url, $parameters, "POST", $headers ); break;
			}

			if( $response && $this->decode_json ){
				$response = json_decode( $response ); 
			}

			return $response; 
		}

		/** 
		* GET wrappwer for provider apis request
		*/
		function get( $url, $parameters = array(), $headers = array() )
		{
			return $this->api( $url, 'GET', $parameters, $headers ); 
		} 

		/** 
		* POST wreapper for provider apis request
		*/
		function post( $url, $parameters = array(), $headers = array() )
		{
			return $this->api( $url, 'POST', $parameters, $headers ); 
		}

		// -- tokens

		public function tokenInfo($accesstoken)
		{
			$params['access_token'] = $this->access_token;
			$response = $this->request( $this->token_info_url, $params );
			return $this->parseRequestResult( $response );
		}

		public function refreshToken( $parameters = array() )
		{
			$params = array(
				"refresh_token" => $this->refresh_token,
				"grant_type"    => "refresh_token"
			);

			foreach($parameters as $k=>$v ){
				$params[$k] = $v; 
			}

			$response = $this->request( $this->token_url, $params, "POST" );
			return $this->parseRequestResult( $response );
		}

		// -- utilities

		private function request( $url, $params=false, $type="GET", $headers = array() )
		{
			if( $type == "GET" ){
				$url = $url . ( strpos( $url, '?' ) ? '&' : '?' ) . http_build_query( $params );
			}

			$this->http_info = array();
			$ch = curl_init();

			$headers[$this->sign_token_name] = $this->access_token;
	    
			curl_setopt($ch, CURLOPT_URL            , $url );
			curl_setopt($ch, CURLOPT_RETURNTRANSFER , 1 );
			curl_setopt($ch, CURLOPT_TIMEOUT        , $this->curl_time_out );
			curl_setopt($ch, CURLOPT_USERAGENT      , $this->curl_useragent );
			curl_setopt($ch, CURLOPT_CONNECTTIMEOUT , $this->curl_connect_time_out );
			curl_setopt($ch, CURLOPT_SSL_VERIFYPEER , $this->curl_ssl_verifypeer );
			curl_setopt($ch, CURLOPT_HTTPHEADER     , array_merge(array('Accept: application/json', 'Content-Type: application/json'), $headers));

			if($this->curl_proxy){
				curl_setopt( $ch, CURLOPT_PROXY        , $this->curl_proxy);
			}

			if( $type == "POST" ){
				curl_setopt($ch, CURLOPT_POST, 1); 
				if($params) curl_setopt( $ch, CURLOPT_POSTFIELDS, json_encode($params) );
			}

			$response = curl_exec($ch);			$this->http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
			$this->http_info = array_merge($this->http_info, curl_getinfo($ch));

			curl_close ($ch);
			return $response; 
		}

		private function parseRequestResult( $result )
		{
			if( json_decode( $result ) ) return json_decode( $result );
			
			parse_str( $result, $ouput ); 

			$result = new StdClass();

			foreach( $ouput as $k => $v )
				$result->$k = $v;

			return $result;
		}
	}
?>
